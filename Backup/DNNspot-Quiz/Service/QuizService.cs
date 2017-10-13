using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using DotNetNuke;
using DNNspot.Quiz.Model;
using WA.Components;
using WA.Extensions;

namespace DNNspot.Quiz
{
    public class QuizService
    {
        protected const string CaptureFieldPrefix = "quiz_capture_";
        DotNetNuke.Data.DataProvider _dataProvider;

        public QuizService()
        {
            _dataProvider = DotNetNuke.Data.DataProvider.Instance();
        }

        public QuizResult ScoreQuiz(Model.Quiz quiz)
        {
            var results = new QuizResult();
            results.Quiz = quiz;

            int totalQuestions = quiz.Questions.Count;
            int correctQuestions = 0;

            foreach(var question in quiz.Questions)
            {
                if (question.Choices.Exists(c => c.IsSelected && c.IsCorrectChoice))
                {
                    correctQuestions++;
                }
            }

            decimal decimalPercent = (((decimal)correctQuestions) / ((decimal)totalQuestions));
            results.PercentScore = (int)(decimalPercent * 100.0m);
            results.IsPassingScore = results.PercentScore >= quiz.PassPercentage;

            var tokens = GetTokens(HttpContext.Current.Request, quiz, results);
            results.Actions.AddRange(ProcessTokensInActions(quiz.Actions.Where(a => a.Condition == QuizCondition.QuizTaken), tokens));
            if(results.IsPassingScore)
            {
                results.Actions.AddRange(ProcessTokensInActions(quiz.Actions.Where(a => a.Condition == QuizCondition.QuizPassed), tokens));
            }

            return results;
        }

        public bool CanUserTakeQuiz(int userId, Model.Quiz quiz)
        {
            if(quiz.LimitPerUser == 0 || userId == -1)
            {
                return true;
            }

            string sql = @"SELECT COUNT(*) FROM DNNspot_Quiz_QuizLog WHERE UserId = @userId AND QuizName = @quizName";
            List<SqlParameter> sqlParams = new List<SqlParameter>()
                                               {                                                    
                                                    new SqlParameter("quizName", quiz.Name),
                                                    new SqlParameter("userId", userId)
                                               };
            int? count = null;
            var reader = _dataProvider.ExecuteSQL(sql, sqlParams.ToArray());
            while(reader.Read())
            {
                count = reader.GetInt32(0);                                
            }            

            return (count < quiz.LimitPerUser);
        }

        public void LogQuiz(Model.Quiz quiz, int moduleId, int userId, string results)
        {
            //---- Log the quiz                       
            
            string sql = @"INSERT INTO DNNspot_Quiz_QuizLog(ModuleId,QuizName,UserId,results) VALUES(@moduleId,@quizName,@userId,@results)";
            List<SqlParameter> sqlParams = new List<SqlParameter>()
                                               {
                                                    new SqlParameter("moduleId", moduleId),                         
                                                    new SqlParameter("quizName", quiz.Name),
                                                    new SqlParameter("userId", userId),
                                                    new SqlParameter("results", results)
                                               };
            _dataProvider.ExecuteSQL(sql, sqlParams.ToArray());
        }

        private List<QuizAction> ProcessTokensInActions(IEnumerable<QuizAction> actions, Dictionary<string,string> tokens)
        {
            var tokenizer = new TokenProcessor("[", "]");
            var processedActions = new List<QuizAction>();
            foreach (var a in actions)
            {
                foreach (var actionEmail in a.Emails)
                {
                    actionEmail.From = tokenizer.ReplaceTokensInString(actionEmail.From, tokens);                    
                    actionEmail.To = tokenizer.ReplaceTokensInString(actionEmail.To, tokens);
                    actionEmail.Cc = tokenizer.ReplaceTokensInString(actionEmail.Cc, tokens);
                    actionEmail.Bcc = tokenizer.ReplaceTokensInString(actionEmail.Bcc, tokens);
                    actionEmail.SubjectTemplate = tokenizer.ReplaceTokensInString(actionEmail.SubjectTemplate, tokens);
                    actionEmail.BodyTemplate = tokenizer.ReplaceTokensInString(actionEmail.BodyTemplate, tokens);
                    //EmailService.SendEmail(from, to, cc, bcc, subject, body);
                }
                a.Message = tokenizer.ReplaceTokensInString(a.Message, tokens);
                processedActions.Add(a);
            }
            return processedActions;
        }

        private Dictionary<string, string> GetTokens(HttpRequest httpRequest, Model.Quiz quiz, Model.QuizResult results)
        {
            var tokens = new Dictionary<string, string>();
            tokens["QUIZ_NAME"] = quiz.Name;
            tokens["QUIZ_PASSFAIL_TEXT"] = results.IsPassingScore ? "PASS" : "FAIL";
            tokens["QUIZ_SCORE"] = results.PercentScore + "%";

            // capture fields
            var formFields = httpRequest.Form;
            foreach (string key in formFields.AllKeys)
            {
                if (key.StartsWith(CaptureFieldPrefix))
                {
                    string name = key.Replace(CaptureFieldPrefix, string.Empty).ToUpper();
                    tokens["CAPTURE_" + name] = formFields.GetValues(key).ToCsv();
                }
            }

            // results HTML
            StringBuilder html = new StringBuilder();
            html.Append("<ol>");
            foreach (var q in results.Quiz.Questions)
            {
                html.AppendFormat(@"<li>{0} <ol style=""list-style-type: lower-alpha;"">", q.Text);
                foreach (var c in q.Choices)
                {
                    html.AppendFormat("<li>{0}{1} {2}</li>", c.IsSelected ? "(selected)" : string.Empty, c.IsCorrectChoice ? "(correct)" : string.Empty, c.Text);
                }
                html.Append("</ol> </li>");
            }
            html.Append("</ol>");
            tokens["QUIZ_RESULTS_HTML"] = html.ToString();

            return tokens;
        }
    }
}