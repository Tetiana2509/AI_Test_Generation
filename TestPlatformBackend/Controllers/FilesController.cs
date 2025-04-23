using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Data;
using TestPlatformBackend.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.Content;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Ppt = DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] int topicId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не завантажено");

            if (!_context.Topics.Any(t => t.Id == topicId))
                return BadRequest("Тема не знайдена");

            var filePath = Path.Combine(_uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var uploadedFile = new FileUpload
            {
                FileName = file.FileName,
                FilePath = filePath,
                TopicId = topicId
            };

            _context.Files.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл завантажено", path = filePath });
        }

        [HttpGet("by-topic/{topicId}")]
        public async Task<IActionResult> GetFilesByTopic(int topicId)
        {
            var files = await _context.Files
                .Where(f => f.TopicId == topicId)
                .ToListAsync();

            return Ok(files);
        }

        [HttpDelete("delete-by-name/{fileName}")]
        public async Task<IActionResult> DeleteFileByName(string fileName)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileName == fileName);
            if (file == null)
                return NotFound(new { message = "Файл не знайдено у базі даних" });

            if (System.IO.File.Exists(file.FilePath))
                System.IO.File.Delete(file.FilePath);

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл видалено" });
        }

        [HttpGet("extract-text/{fileName}")]
        public async Task<IActionResult> ExtractText(string fileName)
        {
            string filePath = Path.Combine(_uploadPath, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Файл не знайдено!" });

            string extractedText;

            if (fileName.EndsWith(".pdf"))
            {
                extractedText = ExtractTextFromPdf(filePath);
            }
            else if (fileName.EndsWith(".txt"))
            {
                extractedText = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            }
            else if (fileName.EndsWith(".docx") || fileName.EndsWith(".doc"))
            {
                extractedText = ExtractTextFromWord(filePath);
            }
            else if (fileName.EndsWith(".pptx") || fileName.EndsWith(".ppt"))
            {
                extractedText = ExtractTextFromPowerPoint(filePath);
            }
            else
            {
                return BadRequest(new { message = "Формат файлу не підтримується!" });
            }

            return Ok(new { text = extractedText });
        }

        private string ExtractTextFromPdf(string filePath)
        {
            var text = new StringBuilder();
            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    text.AppendLine(ContentOrderTextExtractor.GetText(page));
                }
            }
            return text.ToString();
        }

        private string ExtractTextFromWord(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var wordDoc = WordprocessingDocument.Open(stream, false);
            var body = wordDoc.MainDocumentPart?.Document.Body;
            return body?.InnerText ?? "";
        }

        private string ExtractTextFromPowerPoint(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var presentation = PresentationDocument.Open(stream, false);
            var textBuilder = new StringBuilder();

            var slideParts = presentation.PresentationPart?.SlideParts;
            if (slideParts != null)
            {
                foreach (var slidePart in slideParts)
                {
                    var texts = slidePart.Slide.Descendants<A.Text>();
                    foreach (var text in texts)
                    {
                        textBuilder.AppendLine(text.Text);
                    }
                }
            }

            return textBuilder.ToString();
        }
    }
}
