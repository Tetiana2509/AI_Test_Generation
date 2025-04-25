using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly AppDbContext _context;

		public UsersController(AppDbContext context)
		{
			_context = context;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] User user)
		{
			if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
				return BadRequest("��'� ����������� �� ������ ����'�����");

			if (await _context.Users.AnyAsync(u => u.Username == user.Username))
				return BadRequest("���������� � ����� ��'�� ��� ����");

			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			return Ok(user);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] User loginRequest)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password);

			if (user == null)
				return Unauthorized("������ ��'� ����������� ��� ������");

			return Ok(new
			{
				user.Id,
				user.Username,
				user.IsTeacher
			});
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
				return NotFound("����������� �� ��������");
			return Ok(user);
		}

		[HttpGet("me")]
		public async Task<IActionResult> GetCurrentUser([FromQuery] int userId)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user == null)
				return NotFound("����������� �� ��������");
			return Ok(user);
		}
	}
}
