using System;


namespace TestPlatformBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";

        public List<CourseUser> CourseUsers { get; set; } = new();
        public List<TestResult> TestResults { get; set; } = new();
    }
}
