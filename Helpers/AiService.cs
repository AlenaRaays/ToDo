using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToDo.Helpers
{
    public class AiService
    {
        private readonly string _apiKey = "somekey";

        public async Task<string> GetResponseAsync(string prompt)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                    client.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost");
                    client.DefaultRequestHeaders.Add("X-Title", "ToDo App");
                    var requestBody = new
                    {
                        model = "openrouter/owl-alpha",
                        messages = new[]
                        {
                            new { role = "user", content = prompt }
                        }
                    };

                    string json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        return $"Ошибка OpenRouter: {response.StatusCode} - {responseJson}";

                    dynamic result = JsonConvert.DeserializeObject(responseJson);
                    return result.choices[0].message.content.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Критическая ошибка: {ex.Message}";
            }
        }
    }
}