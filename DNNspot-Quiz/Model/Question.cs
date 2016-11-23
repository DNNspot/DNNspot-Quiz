using System.Collections.Generic;
using System.Linq;

namespace DNNspot.Quiz.Model
{
    public class Question
    {
        public string Text { get; set; }
        public List<Choice> Choices { get; set; }
        public List<Message> Messages { get; set; }
        public bool IsCorrect
        {
            get { return this.Choices.Exists(c => c.IsSelected && c.IsCorrectChoice); }
        }

        public Question()
        {
            Choices = new List<Choice>();
            Messages = new List<Message>();
        }        

        public List<Message> IncorrectMessages
        {
            get
            {
                return this.Messages.Where(m => m.Type == MessageType.Incorrect).ToList();
            }
        }
    }
}