using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Controllers
{
    [Route("api/test-results")]
    [ApiController]
    public class TestResultController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestResultController(AppDbContext context)
        {
            _context = context;
        }

        //  Отримати результат конкретного студента для тесту
        [HttpGet("{testId}/user/{userId}")]
        public async Task<IActionResult> GetResultForUser(int testId, int userId)
        {
            var result = await _context.TestResults
                .FirstOrDefaultAsync(r => r.FullTestId == testId && r.UserId == userId);

            if (result == null)
                return Ok(new { hasAttempted = false });

            return Ok(new { hasAttempted = true, score = result.Score });
        }

        //  Перевірити, чи студент вже проходив тест (для блокування повторної спроби)
        [HttpGet("check/{testId}/user/{userId}")]
        public async Task<IActionResult> CheckIfAttempted(int testId, int userId)
        {
            bool alreadySubmitted = await _context.TestResults
                .AnyAsync(r => r.FullTestId == testId && r.UserId == userId);

            return Ok(alreadySubmitted);
        }

        //  Адмін або викладач може отримати всі результати тесту
        [HttpGet("all/{testId}")]
        public async Task<IActionResult> GetAllResults(int testId)
        {
            var results = await _context.TestResults
                .Where(r => r.FullTestId == testId)
                .Include(r => r.User)
                .ToListAsync();

            return Ok(results);
        }

        //  Опціонально: Видалити результат тесту для студента (наприклад, для перездачі)
        [HttpDelete("{testId}/user/{userId}")]
        public async Task<IActionResult> DeleteResult(int testId, int userId)
        {
            var result = await _context.TestResults
                .FirstOrDefaultAsync(r => r.FullTestId == testId && r.UserId == userId);

            if (result == null) return NotFound("Результат не знайдено");

            _context.TestResults.Remove(result);
            await _context.SaveChangesAsync();

            return Ok("Результат видалено");
        }
    }
}