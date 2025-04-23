using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using System.IO;
using System.Threading.Tasks;

[Route("api/test-generation")]
[ApiController]
public class TestGenerationController : ControllerBase
{
    private readonly TestGenerator _testGenerator;
    private readonly AppDbContext _context;
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "SavedTests");

    public TestGenerationController(AppDbContext context)
    {
        _testGenerator = new TestGenerator("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");
        _context = context;
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateTest([FromBody] TestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Текст не може бути порожнім!" });

        if (request.QuestionCount < 1 || request.QuestionCount > 20)
            return BadRequest(new { message = "Кількість питань повинна бути від 1 до 20!" });

        if (!new[] { "простий", "середній", "складний" }.Contains(request.Difficulty?.ToLower()))
            return BadRequest(new { message = "Рівень складності має бути 'простий', 'середній' або 'складний'!" });

        try
        {
            string result = await _testGenerator.GenerateTestFromText(request.Text, request.QuestionCount, request.Difficulty!);
            return Ok(new { message = "Тест згенеровано", test = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Помилка при генерації тесту", error = ex.Message });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveTest([FromBody] SaveTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Назва або зміст тесту не може бути порожнім");

        if (!_context.Topics.Any(t => t.Id == request.TopicId))
            return BadRequest("Тема не знайдена");

        var filePath = Path.Combine(_uploadPath, $"{request.Name}.txt");

        await System.IO.File.WriteAllTextAsync(filePath, request.Content);

        var savedTest = new SavedTest
        {
            TestName = request.Name,
            FilePath = filePath,
            TopicId = request.TopicId
        };

        _context.Tests.Add(savedTest);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Тест збережено", name = request.Name });
    }

    [HttpPost("edit")]
    public async Task<IActionResult> EditTest([FromBody] SaveTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Назва або зміст тесту не може бути порожнім");

        var test = await _context.Tests.FirstOrDefaultAsync(t => t.TestName == request.Name);
        if (test == null)
            return NotFound("Тест не знайдено");

        await System.IO.File.WriteAllTextAsync(test.FilePath, request.Content);

        return Ok(new { message = "Тест оновлено", name = request.Name });
    }

    [HttpDelete("delete/{name}")]
    public async Task<IActionResult> DeleteTest(string name)
    {
        var test = await _context.Tests.FirstOrDefaultAsync(t => t.TestName == name);
        if (test == null)
            return NotFound("Тест не знайдено");

        if (System.IO.File.Exists(test.FilePath))
        {
            System.IO.File.Delete(test.FilePath);
        }

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Тест видалено", name });
    }

    [HttpGet("saved/{topicId}")]
    public async Task<IActionResult> GetSavedTests(int topicId)
    {
        var tests = await _context.Tests
            .Where(t => t.TopicId == topicId)
            .ToListAsync();

        return Ok(tests);
    }

}

public class TestRequest
{
    public string? Text { get; set; }
    public int QuestionCount { get; set; }
    public string? Difficulty { get; set; }
}

public class SaveTestRequest
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int TopicId { get; set; }
}
