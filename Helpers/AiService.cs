using Google.GenAI;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Helpers
{
    class AiService
    {
        private readonly string _apiKey = "AQ.Ab8RN6IZrZUg5uCdYSEQ5oRebfN8Ei96Zyzq2r5iGhqpZ7W-JA";
        public async Task<string> GetResponseAsync(string prompt)
        {
            try
            {
                // Инициализация клиента с ключом
                var client = new Client(apiKey: _apiKey);

                // Обратите внимание: модель gemini-1.5-flash (или новее, если доступна)
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-1.5-flash",
                    contents: prompt
                );

                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                return $"Ошибка подключения к Gemini: {ex.Message}";
            }
        }
    }
}
