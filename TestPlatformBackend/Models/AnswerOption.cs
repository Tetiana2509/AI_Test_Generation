using System;
using System.Text.Json.Serialization;



namespace TestPlatformBackend.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string Text { get; set; } = string.Empty;

        public int QuestionId { get; set; }
        [JsonIgnore]
        public Question Question { get; set; }

    }

}
