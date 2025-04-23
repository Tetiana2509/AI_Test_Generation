using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Controllers
{
    [Route("api/topics")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TopicsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetTopicsForCourse(int courseId)
        {
            var topics = await _context.Topics
                .Where(t => t.CourseId == courseId)
                .ToListAsync();

            return Ok(topics);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] Topic topic)
        {
            if (string.IsNullOrWhiteSpace(topic.TopicName))
                return BadRequest("Назва теми не може бути порожньою");

            if (!_context.Courses.Any(c => c.Id == topic.CourseId))
                return BadRequest("Курс з таким ID не існує");

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            return Ok(topic);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
                return NotFound("Тему не знайдено");

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Тему видалено", id });
        }
    }
}
