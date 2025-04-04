using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestPlatformBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.Content;


[Route("api/test-generation")]
[ApiController]
public class TestGenerationController : ControllerBase
{
    private readonly TestGenerator _testGenerator;
    private readonly AppDbContext _context;
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


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
            return BadRequest(new { message = "Текст не может быть пустым!" });

        if (request.QuestionCount < 1 || request.QuestionCount > 20)
            return BadRequest(new { message = "Кількість питань повинна бути від 1 до 20!" });

        if (!new[] { "простий", "середній", "складний" }.Contains(request.Difficulty.ToLower()))
            return BadRequest(new { message = "Рівень складності має бути 'простий', 'середній' або 'складний'!" });

        string result = await _testGenerator.GenerateTestFromText(request.Text, request.QuestionCount, request.Difficulty);
        return Ok(new { message = "Тест сгенерирован", test = result });
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveTest([FromBody] SaveTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Имя или содержание теста не может быть пустым");

        var fileName = $"{request.Name}.txt";
        var filePath = Path.Combine("SavedTests", fileName);

        // Убедись, что папка SavedTests существует
        Directory.CreateDirectory("SavedTests");

        // Сохраняем файл на диск
        await System.IO.File.WriteAllTextAsync(filePath, request.Content);

        // Сохраняем запись в базу данных
        var savedTest = new SavedTest
        {
            TestName = request.Name,
            FilePath = filePath
        };

        _context.Tests.Add(savedTest);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Тест сохранен", name = request.Name });
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedTests()
    {
        var tests = await _context.Tests.ToListAsync();
        return Ok(tests);
    }


}


// Класс для параметров запроса
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
}



