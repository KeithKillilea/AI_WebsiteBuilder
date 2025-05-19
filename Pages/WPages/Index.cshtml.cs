using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AI_WebsiteBuilder.Data;
using AI_WebsiteBuilder.Models;
using Microsoft.AspNetCore.Mvc;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class IndexModel : PageModel
    {
        private readonly AI_WebsiteBuilder.Data.ApplicationDbContext _context;

        public IndexModel(AI_WebsiteBuilder.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // List of Website Pages created shown in the table
        public IList<WebPage> WebPage { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Admins role users can see all pages created
            if (User.IsInRole("Admin"))
            {
                WebPage = await _context.WebPages
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                // Otherwise Member role users can see only their own pages
                var userEmail = User.Identity?.Name ?? "";
                WebPage = await _context.WebPages
                    .Where(p => p.Member == userEmail)
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();
            }
        }

        // Export the OpenAI generated Website Page as a standalone HTML file
        public async Task<FileResult> OnGetExportAsync(int id)
        {
            // Get the page by ID
            var page = await _context.WebPages.FirstOrDefaultAsync(p => p.ID == id);

            // if not found or content is missing, an empty file is returned
            if (page == null || string.IsNullOrEmpty(page.PageContent))
                return File(Array.Empty<byte>(), "text/plain", "notfound.txt");

            var fileName = $"{page.PageName?.Replace(" ", "_")}.html";

            // Include exported page with Bootstrap & FontAwesome
            string wrappedHtml = page.PageContent;
            if (!page.PageContent.Contains("bootstrap.min.css", StringComparison.OrdinalIgnoreCase))
            {
                wrappedHtml = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>{page.PageName}</title>
                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-4Q6Gf2aSP4eDXB8Miphtr37CMZZQ5oXLH2yaXMJ2w8e2ZtHTl7GptT4jmndRuHDT' crossorigin='anonymous'>
                        <link href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css' rel='stylesheet'>
                    </head>
                    <body>
                        {page.PageContent}
                        <script src='https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/js/bootstrap.bundle.min.js' integrity='sha384-j1CDi7MgGQ12Z7Qab0qlWQ/Qqz24Gc6BM0thvEMVjHnfYGF0rmFCozFSxQBxwHKO' crossorigin='anonymous'></script>
                    </body>
                    </html>";
            }

            // Convert to byte array and return the exported HTML file to download
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(wrappedHtml);
            return File(fileBytes, "text/html", fileName);
        }
    }
}
