using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Controllers
{
	[Route("api/fulltests")]
	[ApiController]
	public class FullTestController : ControllerBase
	{
		private readonly AppDbContext _context;

		public FullTestController(AppDbContext context)
		{
			_context = context;
		}

		// ��������� ����� ����������
		[HttpPost]
		public async Task<IActionResult> CreateFullTest([FromBody] FullTest fullTest)
		{
			_context.FullTests.Add(fullTest);
			await _context.SaveChangesAsync();
			return Ok(fullTest);
		}

		// ����������� ����� ����������
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateFullTest(int id, [FromBody] FullTest updatedTest)
		{
			var existingTest = await _context.FullTests
				.Include(f => f.Questions).ThenInclude(q => q.AnswerOptions)
				.FirstOrDefaultAsync(f => f.Id == id);

			if (existingTest == null)
				return NotFound("���� �� ��������");

			existingTest.Text = updatedTest.Text;
			existingTest.Questions.Clear();
			existingTest.Questions = updatedTest.Questions;

			await _context.SaveChangesAsync();
			return Ok(existingTest);
		}

		//��������� ����� ����������
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFullTest(int id)
		{
			var fullTest = await _context.FullTests.FindAsync(id);
			if (fullTest == null) return NotFound();

			_context.FullTests.Remove(fullTest);
			await _context.SaveChangesAsync();
			return Ok("��������");
		}

		// �������� �� ����� ���� (��� ���������)
		[HttpGet("by-topic/{topicId}")]
		public async Task<IActionResult> GetTestsByTopic(int topicId)
		{
			var tests = await _context.FullTests
				.Where(t => t.TopicId == topicId)
				.ToListAsync();

			return Ok(tests);
		}

		// �������� ���� ������� � ���� ��������� (�����������)
		[HttpGet("{id}")]
		public async Task<IActionResult> GetFullTestById(int id)
		{
			var test = await _context.FullTests
				.Include(t => t.Questions)
				.ThenInclude(q => q.AnswerOptions)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (test == null) return NotFound();
			return Ok(test);
		}

		// �������� ����� ��� ����������� (��� ��� �������)
		[HttpGet("student-view/{id}")]
		public async Task<IActionResult> GetTestForStudent(int id)
		{
			var test = await _context.FullTests
				.Include(t => t.Questions)
				.ThenInclude(q => q.AnswerOptions)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (test == null) return NotFound();

			var result = new
			{
				test.Id,
				test.Text,
				Questions = test.Questions.Select(q => new
				{
					q.Id,
					q.QuestionText,
					AnswerOptions = q.AnswerOptions.Select(a => new { a.Id, a.Text })
				})
			};

			return Ok(result);
		}

		// ³�������� ������� � �������� ��������� ��������
		[HttpPost("submit")]
		public async Task<IActionResult> SubmitTest([FromBody] TestResult submission)
		{
			_context.TestResults.Add(submission);
			await _context.SaveChangesAsync();
			return Ok("��������� ���������");
		}

		[HttpGet("extract-text-from-saved/{fileName}")]
		public async Task<IActionResult> ExtractTextFromSaved(string fileName)
		{
			var filePath = Path.Combine("SavedTests", fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			var text = await System.IO.File.ReadAllTextAsync(filePath);
			return Ok(new { text });
		}

	}
}