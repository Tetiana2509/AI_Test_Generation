using System;


namespace TestPlatformBackend.Models
{
    public class FullTest
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;

        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        public List<Question> Questions { get; set; } = new();
        public List<TestResult> TestResults { get; set; } = new();
    }
}