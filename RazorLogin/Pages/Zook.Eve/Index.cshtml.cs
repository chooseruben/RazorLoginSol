using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Zook.Eve
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<Event> Events { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Correct the Include lambda expression
            Events = await _context.Events
                .Include(e => e.EventEmployeeRep) // Proper syntax for Include
                .ToListAsync();
        }
    }
}
