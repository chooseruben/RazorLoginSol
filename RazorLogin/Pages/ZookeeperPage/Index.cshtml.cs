using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using RazorLogin.Data;

namespace RazorLogin.Pages.ZookeeperPage
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        // Constructor to initialize the database context
        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        // Keep Zookeeper as a List (no changes to other parts of your code)
        public List<Zookeeper> Zookeeper { get; set; } = new List<Zookeeper>();

        public async Task OnGetAsync()
        {
            // Try to fetch the data and handle errors gracefully
            try
            {
                Zookeeper = await _context.Zookeepers
                    .Include(z => z.Employee)  // Load related Employee data
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or show an error (optional)
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }
}
