using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class DetailsModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DetailsModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public RazorLogin.Models.Manager Manager { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.ManagerId == id);
            if (manager == null)
            {
                return NotFound();
            }
            else
            {
                Manager = manager;
            }
            return Page();
        }
    }
}
