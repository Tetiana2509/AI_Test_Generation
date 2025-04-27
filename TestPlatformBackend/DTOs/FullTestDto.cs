using TestPlatformBackend.Models;


namespace TestPlatformBackend.DTOs
{
    public class AnswerOptionDto
    {
        public string Text { get; set; } = string.Empty;
        public double Weight { get; set; }
    }

    public class QuestionDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new();
    }

    public class FullTestDto
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int? TopicId { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }

}
