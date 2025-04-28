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
        "Кожне варіант відповідей має мати вагу правильності, тобто якщо за тестове завдання коштуе одного балу, то всі варіанти відповідей повинні мати вагу, якщо неправильний то вага 0, якщо правильна 1, але якщо у запитанні декілька правельніх відповідей то вага 1 розділяется між правильніми відповідями" +
        "Додай рівень складності до кожного запитання (простий, середній, складний). " +
        "Роби тестові завдання в такому форматі : Які властивості визначають стандартні відступи та розміри боксів в CSS і як вони застосовуються?" +
        "**Рівень складності:** складний" +
        "A) `margin` встановлює внутрішні відступи блоку" +
        "B) `padding` встановлює внутрішні відступи блоку від його зовнішніх меж до контенту(0,3)" +
        "C) `border-width` визначає ширину рамки елемента(0,4)" +
        "D) `height` та `width` встановлюють зовнішні розміри боксу, включаючи рамку та внутрішні/зовнішні відступи" +
        "E) `margin` встановлює зовнішні відступи блоку від його меж до сусідніх елементів або контейнера(0,3)" +
        "**Правильні відповіді:**" +
        "B, C, E " +
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
