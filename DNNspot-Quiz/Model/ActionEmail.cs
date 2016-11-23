namespace DNNspot.Quiz.Model
{
    public class ActionEmail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string SubjectTemplate { get; set; }
        public string BodyTemplate { get; set; }
    }
}