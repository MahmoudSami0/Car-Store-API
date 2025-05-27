using CarStore.Applcation.Services;
using CarStore.InfraStructure.AI;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CarStore.InfraStructure.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly AISettings _aISettings;
        public AIService(HttpClient httpClient, IOptions<AISettings> aISettings)
        {
            _httpClient = httpClient;
            _aISettings = aISettings.Value;
        }

        public async Task<string> GetAIResponse(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = _aISettings.Model, // أو موديل تاني متاح عندك
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, _aISettings.OpenRouterUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _aISettings.ApiKey);
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

