using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.ZookeeperClosings
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "EnclosureId", "EnclosureId");
            return Page();
        }

        [BindProperty]
        public Closing Closing { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a unique random integer for ClosingId
            var random = new Random();
            int newClosingId;
            do
            {
                newClosingId = random.Next(1, int.MaxValue);
            } while (await _context.Closings.AnyAsync(c => c.ClosingId == newClosingId));

            Closing.ClosingId = newClosingId;

            _context.Closings.Add(Closing);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
