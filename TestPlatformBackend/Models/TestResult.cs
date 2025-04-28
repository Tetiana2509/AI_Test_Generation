namespace TestPlatformBackend.Models
{
    public class TestResult
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int FullTestId { get; set; }
        public FullTest FullTest { get; set; } = null!;

        public double Score { get; set; }
        public DateTime SubmittedAt { get; set; }

        public List<AnswerSubmission> AnswerSubmissions { get; set; } = new();
    }
}
