using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AI_WebsiteBuilder.Data;
using AI_WebsiteBuilder.Models;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class EditModel : PageModel
    {
        private readonly AI_WebsiteBuilder.Data.ApplicationDbContext _context;

        public EditModel(AI_WebsiteBuilder.Data.ApplicationDbContext context)
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

            var webpage =  await _context.WebPages.FirstOrDefaultAsync(m => m.ID == id);
            if (webpage == null)
            {
                return NotFound();
            }
            WebPage = webpage;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var originalPage = await _context.WebPages.AsNoTracking().FirstOrDefaultAsync(p => p.ID == WebPage.ID);
            if (originalPage == null)
            {
                return NotFound();
            }

            WebPage.Member = originalPage.Member; // Preserve original Member

            _context.Attach(WebPage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebPageExists(WebPage.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }



        private bool WebPageExists(int id)
        {
            return _context.WebPages.Any(e => e.ID == id);
        }
    }
}
