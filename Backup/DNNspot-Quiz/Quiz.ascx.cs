using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DNNspot.Quiz.Model;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using WA.Components;
using WA.Extensions;

namespace DNNspot.Quiz
{
    public partial class Quiz : ModuleBase, IActionable
    {
        protected Model.Quiz _quiz = new Model.Quiz();
        protected Model.QuizResult _results = new QuizResult();
        protected QuizService _quizService = new QuizService();
        protected const string CaptureFieldPrefix = "quiz_capture_";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                int id = GetId();

                LoadQuiz(id);
            }
        }

        private int GetId()
        {
            HttpCookie imisId = Request.Cookies["IMISSSOID"];
            int id = -1;

            if(imisId != null)
            {
                id = Convert.ToInt32(imisId.Value);
            }
            return id;
        }

        private void LoadQuiz(int id)
        {
            string xmlFile = Paths.QuizFilePath + Convert.ToString(Settings["QuizFile"]);
            _quiz = Model.Quiz.LoadFromXml(xmlFile) ?? new Model.Quiz();

            // check if user has already taken the quiz the Max # of allowed times
            if(!_quizService.CanUserTakeQuiz(id, _quiz))
            {
                pnlTakeQuiz.Visible = false;
                pnlQuizAlreadyTaken.Visible = !pnlTakeQuiz.Visible;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int id = GetId();
            LoadQuiz(id);

            for(int i = 0; i < _quiz.Questions.Count; i++)
            {
                var question = _quiz.Questions[i];
                var responses = new List<string>(Request.Form.GetValues("question" + i) ?? new string[] { });
                if (responses.Count > 0)
                {
                    for (int c = 0; c < question.Choices.Count; c++)
                    {
                        question.Choices[c].IsSelected = responses.Contains(question.Choices[c].Text);
                    }
                }
            }
            
            _results = _quizService.ScoreQuiz(_quiz);
            pnlTakeQuiz.Visible = false;
            pnlQuizResults.Visible = true;

            if(!_quizService.CanUserTakeQuiz(id, _quiz))
            {
                btnSubmitAgain.Visible = false;
            }

            var quizTakenActions = _results.Actions.Where(a => a.Condition == QuizCondition.QuizTaken);

            _quizService.LogQuiz(_quiz, ModuleId, id,
                                 quizTakenActions.FirstOrDefault() != null
                                     ? quizTakenActions.FirstOrDefault().Emails.FirstOrDefault().BodyTemplate
                                     : String.Empty);

            foreach (var a in quizTakenActions)
            {
                foreach (var email in a.Emails)
                {
                    EmailService.SendEmail(email.From, email.To, email.Cc, email.Bcc, email.SubjectTemplate, email.BodyTemplate);
                }
            }

            if(_results.IsPassingScore)
            {
                var quizPassedActions = _results.Actions.Where(a => a.Condition == QuizCondition.QuizPassed);

                if (UserId != -1)
                {
                    bool refreshUserRoles = false;
                    foreach (var action in quizPassedActions)
                    {
                        foreach (var roleToAdd in action.UserRoles)
                        {
                            RoleController roleController = new RoleController();
                            var roleInfo = roleController.GetRoleByName(PortalId, roleToAdd.RoleName);
                            if (roleInfo != null)
                            {
                                DateTime expireDate = roleToAdd.ExpiresAfterDays.HasValue
                                                          ? DateTime.Now.AddDays(roleToAdd.ExpiresAfterDays.Value)
                                                          : DotNetNuke.Common.Utilities.Null.NullDate;
                                roleController.AddUserRole(PortalId, UserId, roleInfo.RoleID, expireDate);
                                refreshUserRoles = true;
                            }
                            else
                            {
                                Exceptions.LogException(
                                    new ModuleLoadException("Tried to add user to non-existent role '" +
                                                            roleToAdd.RoleName + "'"));
                            }
                        }
                    }
                    if (refreshUserRoles)
                    {
                        // Clear the user's cached/stored role membership, will reload on next page cycle
                        PortalSecurity.ClearRoles();
                        DataCache.ClearUserCache(PortalId, UserInfo.Username);

                        // Load the current roles into the user's current context, for use in this page cycle
                        RoleController roleController = new RoleController();
                        UserInfo.Roles = roleController.GetRolesByUser(UserId, PortalId);
                    }
                }
            }
        }



        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();

                actions.Add(
                    GetNextActionID(),
                    "Quiz Settings",
                    ModuleActionType.AddContent,
                    "",
                    "",
                    EditUrl("Settings"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false);

                return actions;

            }
        }
    }
}