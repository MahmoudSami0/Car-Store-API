using CarStore.Applcation.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CarStore.InfraStructure.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "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";
        private const string OpenRouterUrl = "https://openrouter.ai/api/v1/chat/completions";
        public AIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAIResponse(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "qwen/qwen3-235b-a22b", // أو موديل تاني متاح عندك
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, OpenRouterUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = jsonContent;

                var response = await _httpClient.SendAsync(request);

                var raw = await response.Content.ReadAsStringAsync();

                //Console.WriteLine("Response Body: " + raw);

                response.EnsureSuccessStatusCode();

                using var doc = JsonDocument.Parse(raw);

                var reply = doc.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString();

                return reply ?? "لا توجد استجابة من الـ AI.";
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
    }
}

