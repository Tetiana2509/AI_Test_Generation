using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using TestPlatformBackend.DTOs;

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

		// Створення тесту викладачем
		[HttpPost]
		public async Task<IActionResult> CreateFullTest([FromBody] FullTestDto dto)
		{
			var fullTest = new FullTest
			{
				TestName = dto.TestName,
				Text = dto.Text,
				TopicId = dto.TopicId,
				Questions = dto.Questions.Select(q => new Question
				{
					QuestionText = q.QuestionText,
					AnswerOptions = q.AnswerOptions.Select(a => new AnswerOption
					{
						Text = a.Text,
						Weight = a.Weight
					}).ToList()
				}).ToList()
			};

			_context.FullTests.Add(fullTest);
			await _context.SaveChangesAsync();
			return Ok(fullTest);
		}

		// Редагування тесту викладачем
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateFullTest(int id, [FromBody] FullTestDto updatedTest)
		{
			var existingTest = await _context.FullTests
				.Include(f => f.Questions)
				.ThenInclude(q => q.AnswerOptions)
				.FirstOrDefaultAsync(f => f.Id == id);

			if (existingTest == null)
				return NotFound("Тест не знайдено");

			existingTest.TestName = updatedTest.TestName;
			existingTest.Text = updatedTest.Text;

			_context.Questions.RemoveRange(existingTest.Questions);

			existingTest.Questions = updatedTest.Questions.Select(q => new Question
			{
				QuestionText = q.QuestionText,
				AnswerOptions = q.AnswerOptions.Select(a => new AnswerOption
				{
					Text = a.Text,
					Weight = a.Weight
				}).ToList()
			}).ToList();

			await _context.SaveChangesAsync();
			return Ok(existingTest);
		}

		// Видалення тесту викладачем
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFullTest(int id)
		{
			var fullTest = await _context.FullTests.FindAsync(id);
			if (fullTest == null) return NotFound();

			_context.FullTests.Remove(fullTest);
			await _context.SaveChangesAsync();
			return Ok("Видалено");
		}

		// Отримати тести теми
		[HttpGet("by-topic/{topicId}")]
		public async Task<IActionResult> GetTestsByTopic(int topicId)
		{
			var tests = await _context.FullTests
				.Where(t => t.TopicId == topicId)
				.ToListAsync();

			return Ok(tests);
		}

		// Отримати повністю тест
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

		// Отримати тест для студента
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
					AnswerOptions = q.AnswerOptions.Select(a => new
					{
						a.Id,
						a.Text,
						a.Weight
					})
				})
			};

			return Ok(result);
		}

		// Зберегти результат студента
		// ✅ Метод для збереження результату проходження тесту
		[HttpPost("submit")]
		public async Task<IActionResult> SubmitTest([FromBody] SubmitTestDto dto)
		{
			var fullTest = await _context.FullTests
				.Include(t => t.Questions)
				.ThenInclude(q => q.AnswerOptions)
				.FirstOrDefaultAsync(t => t.Id == dto.FullTestId);

			if (fullTest == null)
			{
				return NotFound("Тест не знайдено");
			}

			double totalScore = 0; // 🟣 не int, а double!

			foreach (var submission in dto.AnswerSubmissions)
			{
				var selectedOption = fullTest.Questions
					.SelectMany(q => q.AnswerOptions)
					.FirstOrDefault(a => a.Id == submission.AnswerOptionId);

				if (selectedOption != null)
				{
					totalScore += selectedOption.Weight; // 🟣 не Convert.ToInt32
				}
			}

			var testResult = new TestResult
			{
				UserId = dto.UserId,
				FullTestId = dto.FullTestId,
				SubmittedAt = dto.SubmittedAt,
				Score = Math.Round(totalScore, 2), // 🟣 можно округлить до 2 знаков сразу
				AnswerSubmissions = dto.AnswerSubmissions.Select(a => new AnswerSubmission
				{
					QuestionId = a.QuestionId,
					AnswerOptionId = a.AnswerOptionId
				}).ToList()
			};

			_context.TestResults.Add(testResult);
			await _context.SaveChangesAsync();
			return Ok("Результат збережено!");
		}



		// Отримати бали тестів студента
		[HttpGet("student-scores/{userId}")]
		public async Task<IActionResult> GetStudentScores(int userId)
		{
			var results = await _context.TestResults
				.Where(r => r.UserId == userId)
				.Select(r => new
				{
					r.FullTestId,
					r.Score
				})
				.ToListAsync();

			return Ok(results);
		}

		// ✅ Метод для отримання останнього результату студента по тесту
		[HttpGet("last-result/{testId}")]
		public async Task<IActionResult> GetLastResult(int testId, [FromQuery] int userId)
		{
			var result = await _context.TestResults
				.Where(r => r.FullTestId == testId && r.UserId == userId)
				.OrderByDescending(r => r.SubmittedAt)
				.FirstOrDefaultAsync();

			if (result == null)
				return Ok(null);

			var test = await _context.FullTests
				.Include(t => t.Questions)
				.ThenInclude(q => q.AnswerOptions)
				.FirstOrDefaultAsync(t => t.Id == testId);

			if (test == null)
				return NotFound();

			double maxScore = test.Questions.Sum(q => q.AnswerOptions.Where(a => a.Weight > 0).Sum(a => a.Weight));
			double score = result.Score; // тут double
			double percentage = maxScore > 0 ? score / maxScore * 100 : 0;

			return Ok(new
			{
				score = Math.Round(score, 2),       // 🟣 ставим ОКРУГЛЕНИЕ до 2 знаков
				maxScore = Math.Round(maxScore, 2), // 🟣 max тоже до 2 знаков
				percentage = Math.Round(percentage, 2)
			});
		}

		// 🟣 Отримати всі проходження певного тесту
		[HttpGet("submissions/{testId}")]
		public async Task<IActionResult> GetTestSubmissions(int testId)
		{
			var submissions = await _context.TestResults
				.Where(r => r.FullTestId == testId)
				.OrderByDescending(r => r.SubmittedAt)
				.Select(r => new
				{
					r.Id,
					r.UserId,
					r.User.Email,
					Name = r.User.UserName,
					r.Score,
					r.SubmittedAt
				})
				.ToListAsync();

			return Ok(submissions);
		}


		// 🟣 Видалити одне проходження тесту за ID проходження
		[HttpDelete("submission/{submissionId}")]
		public async Task<IActionResult> DeleteTestSubmission(int submissionId)
		{
			var submission = await _context.TestResults.FindAsync(submissionId);

			if (submission == null)
				return NotFound("Проходження не знайдено");

			_context.TestResults.Remove(submission);
			await _context.SaveChangesAsync();
			return Ok("Проходження видалено");
		}

		// ✅ Отримати текст із збереженого тесту для створення FullTest
		[HttpGet("extract-text-from-saved/{fileName}")]
		public async Task<IActionResult> ExtractTextFromSavedTest(string fileName)
		{
			var filePath = Path.Combine("SavedTests", $"{fileName}.txt");

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound("Файл не знайдено");
			}

			var text = await System.IO.File.ReadAllTextAsync(filePath);
			return Ok(new { text });
		}


	}
}
