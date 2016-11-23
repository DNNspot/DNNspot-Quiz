namespace DNNspot.Quiz.Model
{
    public class CaptureField
    {
        public CaptureFieldType Type { get; set; }
        public string Name { get; set; }
        public string Placeholder { get; set; }
        public bool IsRequired { get; set; }
    }
}