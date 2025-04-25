using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using System.IO;
using System.Threading.Tasks;

[Route("api/dictionary-generation")]
[ApiController]
public class DictionaryGenerationController : ControllerBase
{
    private readonly DictionaryGenerator _dictionaryGenerator;
    private readonly AppDbContext _context;
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "SavedDictionaries");

    public DictionaryGenerationController(AppDbContext context)
    {
        _dictionaryGenerator = new DictionaryGenerator("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");
        _context = context;
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateDictionary([FromBody] DictionaryRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Текст не може бути порожнім!" });

        try
        {
            string result = await _dictionaryGenerator.GenerateDictionaryFromText(request.Text);
            return Ok(new { message = "Словник згенеровано", dictionary = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Помилка при генерації словника", error = ex.Message });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveDictionary([FromBody] SaveDictionaryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Назва або зміст словника не може бути порожнім");

        if (!_context.Topics.Any(t => t.Id == request.TopicId))
            return BadRequest("Тема не знайдена");

        var filePath = Path.Combine(_uploadPath, $"{request.Name}.txt");

        await System.IO.File.WriteAllTextAsync(filePath, request.Content);

        var savedDictionary = new SavedDictionary
        {
            DictionaryName = request.Name,
            FilePath = filePath,
            TopicId = request.TopicId
        };

        _context.Dictionaries.Add(savedDictionary);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Словник збережено", name = request.Name });
    }

    [HttpPost("edit")]
    public async Task<IActionResult> EditDictionary([FromBody] SaveDictionaryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Назва або зміст словника не може бути порожнім");

        var dictionary = await _context.Dictionaries.FirstOrDefaultAsync(d => d.DictionaryName == request.Name);
        if (dictionary == null)
            return NotFound("Словник не знайдено");

        await System.IO.File.WriteAllTextAsync(dictionary.FilePath, request.Content);

        return Ok(new { message = "Словник оновлено", name = request.Name });
    }

    [HttpDelete("delete/{name}")]
    public async Task<IActionResult> DeleteDictionary(string name)
    {
        var dictionary = await _context.Dictionaries.FirstOrDefaultAsync(d => d.DictionaryName == name);
        if (dictionary == null)
            return NotFound("Словник не знайдено");

        if (System.IO.File.Exists(dictionary.FilePath))
        {
            System.IO.File.Delete(dictionary.FilePath);
        }

        _context.Dictionaries.Remove(dictionary);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Словник видалено", name });
    }

    [HttpGet("saved/{topicId}")]
    public async Task<IActionResult> GetSavedDictionaries(int topicId)
    {
        var dictionaries = await _context.Dictionaries
            .Where(d => d.TopicId == topicId)
            .ToListAsync();

        return Ok(dictionaries);
    }

}

public class DictionaryRequest
{
    public string? Text { get; set; }
}

public class SaveDictionaryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int TopicId { get; set; }
}
