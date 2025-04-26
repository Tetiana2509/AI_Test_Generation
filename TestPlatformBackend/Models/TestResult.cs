using System;


namespace TestPlatformBackend.Models
{
    public class TestResult
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int FullTestId { get; set; }
        public FullTest FullTest { get; set; }

        public double Score { get; set; }  // бал, який студент отримав
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}