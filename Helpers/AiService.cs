using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToDo.Helpers
{
    public class AiService
    {

        private readonly string _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        public async Task<string> GetResponseAsync(string prompt)
        {
            try
            {
                string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

                // 2. ВАША ПРАВИЛЬНАЯ ССЫЛКА
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, content);
                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return $"Ошибка API: {response.StatusCode}. Ответ: {responseJson}";
                    }

                    dynamic result = JsonConvert.DeserializeObject(responseJson);
                    return result.candidates[0].content.parts[0].text.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }
    }
}