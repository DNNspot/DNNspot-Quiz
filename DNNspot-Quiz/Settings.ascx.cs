using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;

namespace DNNspot.Quiz
{
    public partial class Settings : ModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                string[] quizFiles = Directory.GetFiles(Paths.ModuleRootFilePath + "QuizFiles");
                ddlQuizFile.DataSource = quizFiles.Select(f => Path.GetFileName(f));
                ddlQuizFile.DataBind();
                ddlQuizFile.Items.Insert(0, new ListItem() { Value = "NewFile", Text = "- New File -" });

                string selectedFile = Convert.ToString(Settings["QuizFile"]);
                if(!string.IsNullOrEmpty(selectedFile))
                {
                    ddlQuizFile.SelectedValue = selectedFile;
                }
                ShowEditUi();
            }
        }

        private void ShowEditUi()
        {                        
            string filepath = Paths.ModuleRootFilePath + @"QuizFiles\" + ddlQuizFile.SelectedValue;
            if(File.Exists(filepath))
            {
                txtFileEditor.Text = File.ReadAllText(filepath);
            }
            else
            {
                txtFileEditor.Text = string.Empty;
            }            
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {            
            string filepath = Paths.ModuleRootFilePath + @"QuizFiles\" + ddlQuizFile.SelectedValue;
            File.WriteAllText(filepath, txtFileEditor.Text);

            ModuleController settings = new ModuleController();
            settings.UpdateModuleSetting(ModuleId, "QuizFile", Path.GetFileName(filepath));

            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }

        protected void lbtnEdit_Click(object sender, EventArgs e)
        {
            ShowEditUi();               
        }

        protected void ddlQuizFile_IndexChanged(object sender, EventArgs e)
        {
            ShowEditUi();
        }
    }
}