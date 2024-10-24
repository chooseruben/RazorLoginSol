using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Zook
{
    public class DetailsModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DetailsModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public Zookeeper Zookeeper { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zookeeper = await _context.Zookeepers.FirstOrDefaultAsync(m => m.ZookeeperId == id);
            if (zookeeper == null)
            {
                return NotFound();
            }
            else
            {
                Zookeeper = zookeeper;
            }
            return Page();
        }
    }
}
