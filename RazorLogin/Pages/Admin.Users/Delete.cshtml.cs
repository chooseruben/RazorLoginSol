using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Users
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AspNetUser AspNetUser { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspnetuser = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == id);

            if (aspnetuser == null)
            {
                return NotFound();
            }
            else
            {
                AspNetUser = aspnetuser;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspnetuser = await _context.AspNetUsers.FindAsync(id);
            if (aspnetuser != null)
            {
                AspNetUser = aspnetuser;
                _context.AspNetUsers.Remove(AspNetUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
