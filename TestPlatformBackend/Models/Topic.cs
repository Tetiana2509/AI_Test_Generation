using System;

public class Topic
{
    public int Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course? Course { get; set; }
}
