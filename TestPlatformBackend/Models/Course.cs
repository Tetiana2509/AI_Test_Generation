using System;


namespace TestPlatformBackend.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public List<Topic> Topics { get; set; } = new();
        public List<CourseUser> CourseUsers { get; set; } = new();
    }
}

