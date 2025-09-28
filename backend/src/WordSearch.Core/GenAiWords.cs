
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
// using OpenAI_API;
// using OpenAI_API.Completions;

public class GenAiWords
{
    private readonly string _apiKey;
    private readonly HttpClient _client;

    public GenAiWords(string apiKey)
    {
        _apiKey = apiKey;
        _client = new HttpClient();
    }

    // public async Task<string[]> GenerateWordsOpenAi(string topic, int count)
    // {

    //     var _api = new OpenAIAPI(_apiKey);

    //     string prompt = $@"
    //         Generate {count} unique words related to '{topic}' for a word search puzzle.
    //         Return the result strictly as a JSON array of strings. For example:
    //         [""apple"", ""banana"", ""cherry""]
    //         Do not include any text outside the array.
    //     ";

    //     var request = new CompletionRequest
    //     {
    //         Prompt = prompt,
    //         Model = "gpt-3.5-turbo",
    //         MaxTokens = 150,
    //         Temperature = 0.7,
    //     };

    //     var result = await _api.Completions.CreateCompletionAsync(request);

    //     if (result == null || result.Completions.Count == 0)
    //         return [];

    //     string jsonText = result.Completions[0].Text.Trim();

    //     try
    //     {
    //         var words = System.Text.Json.JsonSerializer.Deserialize<string[]>(jsonText);
    //         return words ?? [];
    //     }
    //     catch
    //     {
    //         return [];
    //     }
    // }

    public async Task<string[]> GenerateWordsGemini(string topic, int count)
    {
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        // Prompt pedindo array JSON
        var promptText = $"Generate {count} unique words related to the topic keyterm '{topic}' for a word search puzzle. " +
                         "Return strictly as a JSON array of strings, e.g., [\"apple\", \"banana\", \"cherry\"]. " +
                         "Do not include any text outside the array." +
                         "Generate words in the same lenaguage as the key term.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = promptText }
                    }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        content.Headers.Add("x-goog-api-key", _apiKey);

        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseString);

        try
        {
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;

            var arrayText = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            var words = JsonSerializer.Deserialize<string[]>(arrayText ?? "[]");

            if ((words?.Length ?? 0) != count)
                throw new Exception("Quantidade incorreta de palavras retornadas");

            return words ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar resposta do Gemini: {ex.Message}");
            return [];
        }
    }
}

