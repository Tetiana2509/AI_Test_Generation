using System;
using System.Text.Json.Serialization;

namespace TestPlatformBackend.Models
{
    public class AnswerSubmission
    {
        public int Id { get; set; }

        public int TestResultId { get; set; }
        [JsonIgnore]
        public TestResult TestResult { get; set; } = null!;

        public int QuestionId { get; set; }

        public int AnswerOptionId { get; set; }
    }
}
