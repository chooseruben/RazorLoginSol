using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // Ensure this is included
using RazorLogin.Models;

namespace RazorLogin.Pages.EnclosurePage
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Enclosure Enclosure { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Use ToList instead of ToListAsync to avoid async issues
            ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers.ToList(), "ZookeeperId", "ZookeeperId");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the dropdown on postback
                ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers.ToList(), "ZookeeperId", "ZookeeperId");
                return Page();
            }

            _context.Enclosures.Add(Enclosure);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
