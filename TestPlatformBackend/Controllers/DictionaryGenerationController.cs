using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/dictionary-generation")]
[ApiController]
public class DictionaryGenerationController : ControllerBase
{
    private readonly DictionaryGenerator _dictionaryGenerator;
    private readonly AppDbContext _context;
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

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

        string result = await _dictionaryGenerator.GenerateDictionaryFromText(request.Text);
        return Ok(new { message = "Словник згенеровано", dictionary = result });
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveDictionary([FromBody] SaveDictionaryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Назва або зміст словника не може бути порожнім");

        var fileName = $"{request.Name}.txt";
        var filePath = Path.Combine("SavedDictionaries", fileName);

        Directory.CreateDirectory("SavedDictionaries");

        await System.IO.File.WriteAllTextAsync(filePath, request.Content);

        var existingDictionary = await _context.Dictionaries
            .FirstOrDefaultAsync(d => d.DictionaryName == request.Name);

        if (existingDictionary != null)
        {
            existingDictionary.FilePath = filePath;
            _context.Dictionaries.Update(existingDictionary);
        }
        else
        {
            var savedDictionary = new SavedDictionary
            {
                DictionaryName = request.Name,
                FilePath = filePath
            };
            _context.Dictionaries.Add(savedDictionary);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Словник збережено", name = request.Name });
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedDictionaries()
    {
        var dictionaries = await _context.Dictionaries.ToListAsync();
        return Ok(dictionaries);
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

}

public class DictionaryRequest
{
    public string? Text { get; set; }
}

public class SaveDictionaryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
