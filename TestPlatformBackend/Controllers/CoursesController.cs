using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            if (string.IsNullOrWhiteSpace(course.CourseName))
                return BadRequest(new { message = "Назва курсу не може бути порожньою" });

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(course);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Курс не знайдено" });

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Курс видалено", id });
        }
    }
}
