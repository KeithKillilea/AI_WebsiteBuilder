using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class OpenAITestModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OpenAITestModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public string Prompt { get; set; } = "";

        public string? Result { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4-turbo-preview",
                messages = new[]
                {
                    new { role = "system", content = "You are a web design assistant that returns raw HTML and CSS only." },
                    new { role = "user", content = Prompt }
                },
                temperature = 0.7
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            var json = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(json);
                Result = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            }
            else
            {
                Result = $"ERROR: {response.StatusCode}\n\n{json}";
            }

            return Page();
        }

        public void OnGet()
        {
        }
    }
}
