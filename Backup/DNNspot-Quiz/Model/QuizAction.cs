using System.Collections.Generic;

namespace DNNspot.Quiz.Model
{
    public class QuizAction
    {
        public QuizCondition Condition { get; set; }
        public string Message { get; set; }
        public List<RoleInfo> UserRoles { get; set; }
        public List<ActionEmail> Emails { get; set; }

        public QuizAction()
        {
            UserRoles = new List<RoleInfo>();
            Emails = new List<ActionEmail>();
        }
    }
}