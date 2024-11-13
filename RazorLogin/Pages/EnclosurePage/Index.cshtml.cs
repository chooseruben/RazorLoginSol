using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.EnclosurePage
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        // The list of Enclosures to display
        public IList<Enclosure> Enclosure { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Eager load Enclosures with Zookeeper and their related Employee data
            Enclosure = await _context.Enclosures
                .Include(e => e.Zookeeper)            // Include the Zookeeper for each Enclosure
                    .ThenInclude(z => z.Employee)    // Include the related Employee (to get FirstName, LastName)
                .ToListAsync();
        }
    }
}
