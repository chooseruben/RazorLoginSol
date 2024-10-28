using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manager.Manager.Stores
{
    public class DetailsModel : PageModel
    {
        private readonly ZooDbContext _context;

        public DetailsModel(ZooDbContext context)
        {
            _context = context;
        }

        public StoreStatistics Store { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var store = await _context.FoodStores
                .Select(s => new StoreStatistics
                {
                    StoreId = s.FoodStoreId,
                    StoreName = s.FoodStoreName,
                    AverageSalary = s.Employees.Average(e => e.EmployeeSalary),
                    MinimumSalary = s.Employees.Min(e => e.EmployeeSalary),
                    MaximumSalary = s.Employees.Max(e => e.EmployeeSalary)
                })
                .FirstOrDefaultAsync(s => s.StoreId == id);

            if (store == null)
            {
                return NotFound();
            }

            Store = store;
            return Page();
        }

    }
}
