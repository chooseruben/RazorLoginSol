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
            try
            {
                Zookeeper = await _context.Zookeepers
                    .Include(z => z.Employee) // Load related Employee data
                    .OrderBy(z => z.AssignedDepartment) // Sort by Department
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

    }
}
