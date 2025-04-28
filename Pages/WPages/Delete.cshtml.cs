using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AI_WebsiteBuilder.Data;
using AI_WebsiteBuilder.Models;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class DeleteModel : PageModel
    {
        private readonly AI_WebsiteBuilder.Data.ApplicationDbContext _context;

        public DeleteModel(AI_WebsiteBuilder.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebPage WebPage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webpage = await _context.WebPages.FirstOrDefaultAsync(m => m.ID == id);

            if (webpage is not null)
            {
                WebPage = webpage;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webpage = await _context.WebPages.FindAsync(id);
            if (webpage != null)
            {
                WebPage = webpage;
                _context.WebPages.Remove(WebPage);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
