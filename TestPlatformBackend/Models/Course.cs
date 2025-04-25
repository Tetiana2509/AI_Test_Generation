using System;

public class Course
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;

    public int CreatorId { get; set; }
    public User? Creator { get; set; }

    public List<Topic> Topics { get; set; } = new();
    public ICollection<CourseUser> Users { get; set; } = new List<CourseUser>(); 
}

