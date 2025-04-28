using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AI_WebsiteBuilder.Data;
using AI_WebsiteBuilder.Models;

namespace AI_WebsiteBuilder.Pages.WPages
{
    public class IndexModel : PageModel
    {
        private readonly AI_WebsiteBuilder.Data.ApplicationDbContext _context;

        public IndexModel(AI_WebsiteBuilder.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WebPage> WebPage { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Admin"))
            {
                WebPage = await _context.WebPages
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                var userEmail = User.Identity?.Name ?? "";
                WebPage = await _context.WebPages
                    .Where(p => p.Member == userEmail)
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();
            }
        }
    }
}

