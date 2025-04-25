using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Controllers
{
	[ApiController]
	[Route("api/course-users")]
	public class CourseUsersController : ControllerBase
	{
		private readonly AppDbContext _context;

		public CourseUsersController(AppDbContext context)
		{
			_context = context;
		}

		[HttpPost("join")]
		public async Task<IActionResult> JoinCourse([FromBody] CourseUser join)
		{
			if (!_context.Courses.Any(c => c.Id == join.CourseId))
				return BadRequest("Курс не знайдено");

			if (!_context.Users.Any(u => u.Id == join.UserId))
				return BadRequest("Користувача не знайдено");

			if (_context.CourseUsers.Any(cu => cu.CourseId == join.CourseId && cu.UserId == join.UserId))
				return BadRequest("Вже приєднаний до курсу");

			_context.CourseUsers.Add(join);
			await _context.SaveChangesAsync();

			return Ok("Успішно приєднано до курсу");
		}

		[HttpGet("by-user/{userId}")]
		public async Task<IActionResult> GetUserCourses(int userId)
		{
			var courses = await _context.CourseUsers
				.Where(cu => cu.UserId == userId)
				.Select(cu => cu.Course)
				.ToListAsync();

			return Ok(courses);
		}
	}

}
