using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using RazorLogin.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.EnclosurePage
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        // List to hold the ViewModel data
        public IList<EnclosureViewModel> Enclosures { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Query the database and map the data to the ViewModel
            Enclosures = await _context.Enclosures
                .Include(e => e.Zookeeper) // Load related Zookeeper data
                .Select(e => new EnclosureViewModel
                {
                    EnclosureId = e.EnclosureId,
                    EnclosureName = e.EnclosureName,
                    EnclosureDepartment = e.EnclosureDepartment,
                    OccupancyStatus = e.OccupancyStatus,
                    EnclosureOpenTime = e.EnclosureOpenTime,
                    EnclosureCloseTime = e.EnclosureCloseTime,
                    IsClosed = e.Closings.Any(c => c.EnclosureId == e.EnclosureId), // Check if closed
                    ZookeeperId = e.Zookeeper != null ? e.Zookeeper.ZookeeperId : (int?)null // Nullable if unassigned
                })
                .ToListAsync();
        }

        // Helper method to determine if an enclosure is closed
        public bool IsEnclosureClosed(EnclosureViewModel enclosure)
        {
            return enclosure.IsClosed;
        }
    }
}
