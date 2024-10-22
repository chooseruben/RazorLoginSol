using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Deps
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dependant Dependant { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dependant = await _context.Dependants.FirstOrDefaultAsync(m => m.DepndantId == id);

            if (dependant == null)
            {
                return NotFound();
            }
            else
            {
                Dependant = dependant;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dependant = await _context.Dependants.FindAsync(id);
            if (dependant != null)
            {
                Dependant = dependant;
                _context.Dependants.Remove(Dependant);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
