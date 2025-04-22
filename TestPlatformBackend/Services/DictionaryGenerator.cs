using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DictionaryGenerator
{
    private readonly OpenAIService _openAiService;

    public DictionaryGenerator(string apiKey)
    {
        _openAiService = new OpenAIService(new OpenAiOptions { ApiKey = apiKey });
    }

    public async Task<string> GenerateDictionaryFromText(string text)
    {
        string prompt =
        $"Прочитай наступний текст і створи словник на основі прочитаного тексту. " +
        $"Ось текст :\n{text}";

        var chatRequest = new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                new ChatMessage("system", "Ти створюєш словник на основі прочитаного тексту."),
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
            return "Помилка генерації словника: " + response.Error?.Message;
        }
    }
}

