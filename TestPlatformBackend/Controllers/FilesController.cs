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

namespace TestPlatformBackend.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public FilesController(AppDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        // 🔹 Загрузка файла
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не загружен");

            var filePath = Path.Combine(_uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var uploadedFile = new FileUpload
            {
                FileName = file.FileName,
                FilePath = filePath
            };

            _context.Files.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл загружен", path = filePath });
        }

        // 🔹 Получить список всех файлов
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await _context.Files.ToListAsync();
            return Ok(files);
        }

        // 🔹 Удаление по имени
        [HttpDelete("delete-by-name/{fileName}")]
        public async Task<IActionResult> DeleteFileByName(string fileName)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileName == fileName);
            if (file == null)
                return NotFound(new { message = "Файл не найден в базе" });

            if (System.IO.File.Exists(file.FilePath))
                System.IO.File.Delete(file.FilePath);

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл удален" });
        }

        // 🔹 Удаление по ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileByID(int id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
                return NotFound(new { message = "Файл не найден в базе" });

            if (System.IO.File.Exists(file.FilePath))
                System.IO.File.Delete(file.FilePath);

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл удален" });
        }

        // 🔹 Извлечение текста
        [HttpGet("extract-text/{fileName}")]
        public async Task<IActionResult> ExtractText(string fileName)
        {
            string filePath = Path.Combine(_uploadPath, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Файл не найден!" });

            string extractedText;

            if (fileName.EndsWith(".pdf"))
            {
                extractedText = ExtractTextFromPdf(filePath);
            }
            else if (fileName.EndsWith(".txt"))
            {
                extractedText = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            }
            else
            {
                return BadRequest(new { message = "Формат файла не поддерживается!" });
            }

            return Ok(new { text = extractedText });
        }

        // Вспомогательный метод для PDF
        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();
            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (Page page in document.GetPages())
                {
                    text.AppendLine(ContentOrderTextExtractor.GetText(page));
                }
            }
            return text.ToString();
        }
    }
}
