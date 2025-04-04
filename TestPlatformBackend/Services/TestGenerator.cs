using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestGenerator
{
    private readonly OpenAIService _openAiService;

    public TestGenerator(string apiKey)
    {
        _openAiService = new OpenAIService(new OpenAiOptions { ApiKey = apiKey });
    }

    public async Task<string> GenerateTestFromText(string text, int questionCount, string difficulty)
    {
        string prompt =
        $"Прочитай наступний текст і створи {questionCount} запитань для тесту. " +
        $"Рівень складності: {difficulty}. " +
        "Кожне запитання повинно мати 5 варіантів відповідей, де одна або кілька відповідей правильні. " +
        "Додай рівень складності до кожного запитання (простий, середній, складний). " +
        $"Ось текст лекції:\n{text}";

        var chatRequest = new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                new ChatMessage("system", "Ти створюєш тест на основі прочитаного тексту."),
                new ChatMessage("user", prompt)
            },
            Model = Models.Gpt_4_turbo
        };

        var response = await _openAiService.ChatCompletion.CreateCompletion(chatRequest);

        if (response.Successful)
        {
            return response.Choices[0].Message.Content;
        }
        else
        {
            return "Помилка генерації тесту: " + response.Error?.Message;
        }
    }
}
