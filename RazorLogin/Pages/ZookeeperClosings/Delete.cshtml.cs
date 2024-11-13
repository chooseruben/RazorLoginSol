using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.ZookeeperClosings
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Closing Closing { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var closing = await _context.Closings.FirstOrDefaultAsync(m => m.ClosingId == id);

            if (closing == null)
            {
                return NotFound();
            }
            else
            {
                Closing = closing;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var closing = await _context.Closings.FindAsync(id);
            if (closing != null)
            {
                Closing = closing;
                _context.Closings.Remove(Closing);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
