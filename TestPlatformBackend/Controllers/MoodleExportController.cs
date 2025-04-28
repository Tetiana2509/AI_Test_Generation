using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TestPlatformBackend.Controllers
{
    [Route("api/moodle-export")]
    [ApiController]
    public class MoodleExportController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateMoodleXml([FromBody] string testText)
        {
            try
            {
                var questions = ParseTestText(testText);

                var quiz = new XElement("quiz",
                    questions.Select(q => new XElement("question", new XAttribute("type", "multichoice"),
                        new XElement("name", new XElement("text", q.Title)),
                        new XElement("questiontext", new XAttribute("format", "html"),
                            new XElement("text", $"<![CDATA[{q.Title}]]>")
                        ),
                        new XElement("single", q.IsSingleAnswer ? "true" : "false"),
                        new XElement("shuffleanswers", "true"),
                        new XElement("answernumbering", "abc"),
                        q.Answers.Select(a =>
                            new XElement("answer", new XAttribute("fraction", (a.Weight * 100).ToString("F0")),
                                new XElement("text", $"<![CDATA[{a.Text}]]>")
                            )
                        )
                    ))
                );

                var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), quiz);
                var xmlString = doc.ToString();

                var bytes = Encoding.UTF8.GetBytes(xmlString);
                return File(bytes, "application/xml", "moodle_questions.xml");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private List<QuestionModel> ParseTestText(string text)
        {
            var questions = new List<QuestionModel>();

            var blocks = Regex.Split(text.Trim(), @"(?=\*\*Запитання\s+\d+:)");

            foreach (var block in blocks)
            {
                if (string.IsNullOrWhiteSpace(block)) continue;

                var titleMatch = Regex.Match(block, @"\*\*Запитання\s+\d+:\*\*\s*(.+?)\s*\*\*", RegexOptions.Singleline);
                var title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : "Без назви";

                var answers = new List<AnswerModel>();

                var answerMatches = Regex.Matches(block, @"([A-E])\)\s*(.+?)\s*\(([\d\.]+)\)");

                foreach (Match match in answerMatches)
                {
                    answers.Add(new AnswerModel
                    {
                        Text = match.Groups[2].Value.Trim(),
                        Weight = double.Parse(match.Groups[3].Value.Replace(",", "."))
                    });
                }

                bool isSingleAnswer = answers.Count(a => a.Weight > 0) == 1;

                questions.Add(new QuestionModel
                {
                    Title = title,
                    Answers = answers,
                    IsSingleAnswer = isSingleAnswer
                });
            }

            return questions;
        }

        private class QuestionModel
        {
            public string Title { get; set; }
            public List<AnswerModel> Answers { get; set; }
            public bool IsSingleAnswer { get; set; }
        }

        private class AnswerModel
        {
            public string Text { get; set; }
            public double Weight { get; set; }
        }
    }
}
