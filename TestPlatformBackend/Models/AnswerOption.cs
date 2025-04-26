using System;


namespace TestPlatformBackend.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public double Weight { get; set; }  // від 0 до 1 або інші значення

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

}
