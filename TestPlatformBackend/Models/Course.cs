using System;

public class Course
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<Topic> Topics { get; set; } = new();
}

