using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using TestPlatformBackend.DTOs;


namespace TestPlatformBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = dto.Password, // для простоти; краще хешувати
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == dto.Password);

            if (user == null)
                return Unauthorized(new { message = "Невірний email або пароль" });

            return Ok(new { user.Id, user.Role });
        }


        // POST: api/Users/join-course
        [HttpPost("join-course")]
        public async Task<IActionResult> JoinCourse([FromBody] JoinCourseDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            var course = await _context.Courses.FindAsync(dto.CourseId);

            if (user == null || course == null)
                return NotFound("User or Course not found");

            var existing = _context.CourseUsers
                .FirstOrDefault(cu => cu.UserId == dto.UserId && cu.CourseId == dto.CourseId);

            if (existing != null)
                return BadRequest("Already joined");

            _context.CourseUsers.Add(new CourseUser
            {
                UserId = dto.UserId,
                CourseId = dto.CourseId
            });

            await _context.SaveChangesAsync();

            return Ok("Joined successfully");
        }

        // GET: api/Users/{userId}/courses
        [HttpGet("{userId}/courses")]
        public async Task<IActionResult> GetUserCourses(int userId)
        {
            var userCourses = await _context.CourseUsers
                .Where(cu => cu.UserId == userId)
                .Include(cu => cu.Course)
                .Select(cu => cu.Course)
                .ToListAsync();

            return Ok(userCourses);
        }
    }

}
