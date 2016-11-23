using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DNNspot.Quiz.Model
{
    public class Quiz
    {
        public string Name { get; set; }
        public int PassPercentage { get; set; }
        public bool DisplayScore { get; set; }
        public bool DisplayHints { get; set; }
        public int LimitPerUser { get; set; }

        public List<CaptureField> CaptureFields { get; set; }        
        public List<Question> Questions { get; set; }        
        public List<QuizAction> Actions { get; set; }
        //public List<Setting> Settings { get; set; }

        public Quiz()
        {
            Name = string.Empty;
            PassPercentage = 0;
            CaptureFields = new List<CaptureField>();
            Questions = new List<Question>();
            Actions = new List<QuizAction>();
            DisplayScore = true;
            DisplayHints = true;
            LimitPerUser = 0;
        }

        public static Quiz LoadFromXml(string xmlFile)
        {            
            if (File.Exists(xmlFile))
            {
                var quiz = new Quiz();

                var xml = XElement.Load(xmlFile);
                quiz.Name = xml.Attribute("name").Value;
                quiz.PassPercentage = WA.Parser.ToInt(xml.Attribute("passPercentage").Value).GetValueOrDefault();
                quiz.DisplayScore = WA.Parser.ToBool(xml.GetAttributeValue("displayScore")).GetValueOrDefault(true);
                quiz.DisplayHints = WA.Parser.ToBool(xml.GetAttributeValue("displayHints")).GetValueOrDefault(true);
                quiz.LimitPerUser = WA.Parser.ToInt(xml.GetAttributeValue("limitPerUser")).GetValueOrDefault(0);

                xml.Element("capturefields")
                    .Elements("field").ToList().ForEach(f => 
                        quiz.CaptureFields.Add(new CaptureField()
                                            {
                                                Type = WA.Enum<CaptureFieldType>.TryParseOrDefault(f.GetAttributeValue("type"), CaptureFieldType.Text),
                                                Name = f.Element("name").Value,
                                                Placeholder = f.GetElementValue("placeholder"),
                                                IsRequired = WA.Parser.ToBool(f.GetElementValue("required")).GetValueOrDefault(false)
                                            }));
                xml.Element("questions")
                    .Elements("question").ToList().ForEach(q =>
                                    quiz.Questions.Add(new Question()
                                           {
                                               Text = q.Element("text").Value,
                                               Choices = q.Element("choices").Elements("choice")
                                                            .Select(c => new Choice()
                                                                             {
                                                                                 Text = c.Value,
                                                                                 IsCorrectChoice = WA.Parser.ToBool(c.GetAttributeValue("correct")).GetValueOrDefault(false)
                                                                             }).ToList(),
                                               Messages = q.Element("messages").Elements("message")
                                                            .Select(r => new Message()
                                                                             {
                                                                                 Type = WA.Enum<MessageType>.TryParseOrDefault(r.GetAttributeValue("type"), MessageType.Incorrect),
                                                                                 Text = r.Value
                                                                             }).ToList()
                                           }));

                xml.Element("actions")
                    .Elements("action").ToList().ForEach(a =>
                            quiz.Actions.Add(new QuizAction()
                                                 {
                                                     Condition = WA.Enum<QuizCondition>.TryParseOrDefault(a.GetAttributeValue("condition"), QuizCondition.Any),
                                                     Message = a.GetElementValue("message"),
                                                     UserRoles = a.Element("roles") == null ? new List<RoleInfo>() : a.Element("roles")
                                                                    .Elements("role").Select(r => new RoleInfo()
                                                                                                {
                                                                                                    RoleName = r.Value,
                                                                                                    ExpiresAfterDays = WA.Parser.ToInt(r.GetAttributeValue("expiresAfterDays"))
                                                                                                }).ToList(),
                                                     Emails = a.Element("emails") == null ? new List<ActionEmail>() : a.Element("emails")
                                                                     .Elements("email").Select(e => new ActionEmail()
                                                                                                {
                                                                                                    From = e.GetElementValue("from"),
                                                                                                    To = e.GetElementValue("to"),
                                                                                                    Cc = e.GetElementValue("cc"),
                                                                                                    Bcc = e.GetElementValue("bcc"),
                                                                                                    SubjectTemplate = e.GetElementValue("subject"),
                                                                                                    BodyTemplate = e.GetElementValue("body"),
                                                                                                }).ToList()
                                                 }));
                //xml.Element("settings")
                //    .Elements("setting").ToList().ForEach(s =>
                //        quiz.Settings.Add(new Setting()
                //                              {
                //                                  Name = s.GetElementValue("name"),
                //                                  Value = s.GetElementValue("value"),
                //                              })
                //    );

                return quiz;
            }   
            return null;            
        }
    }

    //public class Setting
    //{
    //    public string Name { get; set; }
    //    public string Value { get; set; }
    //}
}