namespace TestPlatformBackend.DTOs
{
    public class CreateCourseDto
    {
        public string CourseName { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
    }
}
