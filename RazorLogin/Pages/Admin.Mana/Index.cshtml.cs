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
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<RazorLogin.Models.Manager> Manager { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Manager = await _context.Managers
                .Include(m => m.Employee).ToListAsync();
        }
    }
}
