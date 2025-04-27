using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

namespace TestPlatformBackend.Controllers
{
    [Route("api/moodle-export")]
    [ApiController]
    public class MoodleExportController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateMoodleTestXml([FromBody] string testText)
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
                return File(bytes, "application/xml", "moodle_test.xml");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("generate-dictionary")]
        public IActionResult GenerateMoodleDictionaryXml([FromBody] string dictionaryText)
        {
            try
            {
                var entries = ParseDictionaryText(dictionaryText);

                var glossary = new XElement("glossary",
                    entries.Select(entry => new XElement("entry",
                        new XElement("concept", entry.Term),
                        new XElement("definition", entry.Definition)
                    ))
                );

                var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), glossary);
                var xmlString = doc.ToString();

                var bytes = Encoding.UTF8.GetBytes(xmlString);
                return File(bytes, "application/xml", "moodle_dictionary.xml");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        

        private List<QuestionModel> ParseTestText(string text)
        {
            var questions = new List<QuestionModel>();

            var blocks = System.Text.RegularExpressions.Regex.Split(text.Trim(), @"(?=\*\*Запитання\s+\d+:)");

            foreach (var block in blocks)
            {
                if (string.IsNullOrWhiteSpace(block)) continue;

                var titleMatch = System.Text.RegularExpressions.Regex.Match(block, @"\*\*Запитання\s+\d+:\*\*\s*(.+?)\s*\*\*", System.Text.RegularExpressions.RegexOptions.Singleline);
                var title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : "Без назви";

                var answers = new List<AnswerModel>();

                var answerMatches = System.Text.RegularExpressions.Regex.Matches(block, @"([A-E])\)\s*(.+?)\s*\(([\d\.]+)\)");

                foreach (System.Text.RegularExpressions.Match match in answerMatches)
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

        private List<DictionaryEntryModel> ParseDictionaryText(string text)
        {
            var entries = new List<DictionaryEntryModel>();

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { " - " }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    entries.Add(new DictionaryEntryModel
                    {
                        Term = parts[0].Trim(),
                        Definition = parts[1].Trim()
                    });
                }
            }

            return entries;
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

        private class DictionaryEntryModel
        {
            public string Term { get; set; }
            public string Definition { get; set; }
        }
    }
}
