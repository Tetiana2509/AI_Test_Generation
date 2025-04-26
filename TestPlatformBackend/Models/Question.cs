using System;


namespace TestPlatformBackend.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;

        public int FullTestId { get; set; }
        public FullTest FullTest { get; set; }

        public List<AnswerOption> AnswerOptions { get; set; } = new();
    }
}
