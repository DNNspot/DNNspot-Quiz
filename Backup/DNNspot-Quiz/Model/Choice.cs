namespace DNNspot.Quiz.Model
{
    public class Choice
    {
        public string Text { get; set; }
        public bool IsCorrectChoice { get; set; }
        public bool IsSelected { get; set; }

        public Choice()
        {                        
            IsSelected = false;
        }
    }
}