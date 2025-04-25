using System;

public class SavedTest
{
    public int Id { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public Topic? Topic { get; set; }

    public int UserId { get; set; }              
    public User? User { get; set; }               
}
