using System;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsTeacher { get; set; } = false; 

    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<CourseUser> Courses { get; set; } = new List<CourseUser>();
}
