using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using TestPlatformBackend.DTOs;


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

        // 🟢 Створення курсу викладачем
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CourseName))
                return BadRequest(new { message = "Назва курсу не може бути порожньою" });

            var user = await _context.Users.FindAsync(dto.CreatedByUserId);
            if (user == null || user.Role != "Teacher")
                return BadRequest("Недійсний викладач");

            var course = new Course
            {
                CourseName = dto.CourseName,
                CreatedByUserId = dto.CreatedByUserId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }


        // 🔵 Приєднання по ID курсу (код курсу)
        [HttpPost("join")]
        public async Task<IActionResult> JoinCourse(int userId, int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            var user = await _context.Users.FindAsync(userId);

            if (course == null || user == null)
                return NotFound("Курс або користувач не знайдено");

            bool alreadyJoined = await _context.CourseUsers
                .AnyAsync(cu => cu.CourseId == courseId && cu.UserId == userId);

            if (alreadyJoined)
                return BadRequest("Ви вже приєднані до курсу");

            _context.CourseUsers.Add(new CourseUser
            {
                CourseId = courseId,
                UserId = userId
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Успішно приєднано до курсу" });
        }

        // 🟡 Курси, до яких приєднаний користувач
        [HttpGet("joined/{userId}")]
        public async Task<IActionResult> GetJoinedCourses(int userId)
        {
            var joinedCourses = await _context.CourseUsers
                .Where(cu => cu.UserId == userId)
                .Include(cu => cu.Course)
                .Select(cu => cu.Course)
                .ToListAsync();

            return Ok(joinedCourses);
        }

        // 🔴 Викладач бачить свої курси
        [HttpGet("created/{userId}")]
        public async Task<IActionResult> GetCreatedCourses(int userId)
        {
            var courses = await _context.Courses
                .Where(c => c.CreatedByUserId == userId)
                .ToListAsync();

            return Ok(courses);
        }

        // 🗑 Видалення
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

        // 🔻 Вихід з курсу студентом
        [HttpDelete("leave")]
        public async Task<IActionResult> LeaveCourse(int userId, int courseId)
        {
            var record = await _context.CourseUsers
                .FirstOrDefaultAsync(cu => cu.UserId == userId && cu.CourseId == courseId);

            if (record == null)
                return NotFound("Запис не знайдено");

            _context.CourseUsers.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ви покинули курс" });
        }

    }

}
