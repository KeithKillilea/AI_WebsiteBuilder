using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AI_WebsiteBuilder.Data;
using AI_WebsiteBuilder.Models;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // Default gallery image URLs for replacing <img> tags in generated HTML, otherwise broken images will be shown
        private static readonly string[] GalleryImages = new[]
        {
            "https://images.unsplash.com/photo-1523275335684-37898b6baf30?q=80&w=1999&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
            "https://images.unsplash.com/photo-1531297484001-80022131f5a1?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.1.0",
            "https://images.unsplash.com/photo-1491933382434-500287f9b54b?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
            "https://images.unsplash.com/photo-1542744095-291d1f67b221?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.1.0",
            "https://images.unsplash.com/photo-1498050108023-c5249f4df085?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.1.0",
            "https://images.unsplash.com/photo-1543269865-cbf427effbad?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.1.0"
        };

        public CreateModel(ApplicationDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public WebPage WebPage { get; set; } = default!;

        public IActionResult OnGet()
        {
            WebPage = new WebPage
            {
                Member = User.Identity?.Name ?? "unknown@domain.com"
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the User Account and timestamp
            WebPage.Member = User.Identity?.Name ?? "unknown@domain.com";
            WebPage.CreatedAt = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return Page(); // Stop if form validation fails
            }

            // Construct user prompt for OpenAI using business details
            var businessPrompt = $"Create a modern, mobile-friendly landing page for a business website. " +
                                 $"Business type: {WebPage.PageDescription}. " +
                                 $"Business name: {WebPage.PageName}. " +
                                 $"User description: {WebPage.PageContent}";

            var apiKey = _configuration["OpenAI:ApiKey"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Build the OpenAI chat request with fixed system prompt and user input
            var requestBody = new
            {
                model = "gpt-4-turbo-preview",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = @"You are a professional web design assistant. Generate a full standalone HTML5 landing page for a modern website.
                                    Include proper <html>, <head>, and <body> tags.
                                    Use Bootstrap 5.3.6 from the jsDelivr CDN:
                                    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/css/bootstrap.min.css' rel='stylesheet'>
                                    <script src='https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/js/bootstrap.bundle.min.js'></script>
                                    Embed all custom CSS in a <style> tag.
                                    Do not use duplicate <meta> or <title> tags.
                                    The page design should be colourful with a gradient color palette.
                                    The Header should include the name on the left and Menu on the right.
                                    The Menu will have these options: Home, About, Services, Gallery, Pricing and Contact Us.
                                    The page will include these sections matching the menu.
                                    Each menu item should link to its section via anchor tags.
                                    Use a mix of multiple rows and columns in each section.
                                    Populate with realistic placeholder content (a few hundred words per section).
                                    The Gallery section should include up to 6 images with a Bootstrap carousel.
                                    The Pricing section should include 2 tables with sample packages.
                                    The footer should include the name, links, and a tagline."
                    },
                    new { role = "user", content = businessPrompt }
                },
                temperature = 0.7
            };

            // Send the request to OpenAI to get generating HTML content
            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "OpenAI failed to generate page content.");
                return Page();
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resultJson);

            // Extract the HTML content from OpenAI response
            var rawHtml = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // Clean the markdown wrappers and trim, so OpenAI does not in its response include ```html
            var cleanHtml = rawHtml
                .Replace("```html", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Trim() ?? string.Empty; // Ensure cleanHtml is not null - GitHub AI Recommended from Scan

            // Truncate anything after </html> in case of OpenAI random extra output of info
            var htmlEndIndex = cleanHtml.IndexOf("</html>", StringComparison.OrdinalIgnoreCase);
            if (htmlEndIndex > -1)
            {
                cleanHtml = cleanHtml.Substring(0, htmlEndIndex + 7); // Include </html>
            }

            // Replace <img> tags with known Unsplash image URLs, otherwise broken images will be shown
            var random = new Random();
            int imageIndex = 0;

            cleanHtml = Regex.Replace(cleanHtml, @"<img\s+[^>]*src=['""][^'""]*['""]", match =>
            {
                string imageUrl = GalleryImages[imageIndex % GalleryImages.Length];
                imageIndex++;
                return $"<img src=\"{imageUrl}\"";
            }, RegexOptions.IgnoreCase);

            // Save the OpenAI generated HTML to database Page Content table column
            WebPage.PageContent = cleanHtml;

            _context.WebPages.Add(WebPage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
