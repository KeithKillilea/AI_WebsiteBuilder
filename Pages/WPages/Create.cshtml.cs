using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AI_WebsiteBuilder.Data;
using AI_WebsiteBuilder.Models;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class CreateModel : PageModel
    {
        private readonly AI_WebsiteBuilder.Data.ApplicationDbContext _context;

        public CreateModel(AI_WebsiteBuilder.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebPage WebPage { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Prefill Member when form loads
            WebPage = new WebPage
            {
                Member = User.Identity?.Name ?? "unknown@domain.com"
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            WebPage.CreatedAt = DateTime.Now;

            _context.WebPages.Add(WebPage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
