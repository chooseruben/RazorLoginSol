using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RazorLogin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manager.Manager.Stores
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context; 
        }

        public List<StoreStatistics> StoreStatistics { get; set; } = new();

        public async Task OnGetAsync()
        {
            StoreStatistics = await _context.FoodStores
                .Select(s => new StoreStatistics
                {
                    StoreId = s.FoodStoreId,
                    StoreName = s.FoodStoreName,
                    AverageSalary = s.Employees.Average(e => e.EmployeeSalary),
                    MinimumSalary = s.Employees.Min(e => e.EmployeeSalary),
                    MaximumSalary = s.Employees.Max(e => e.EmployeeSalary),
                })
                .ToListAsync();
        }
    }

    public class StoreStatistics
    {
        public int StoreId {  get; set; }
        public string StoreName { get; set; }
        public double? AverageSalary { get; set; }
        public int? MinimumSalary { get; set; }
        public int? MaximumSalary { get; set; }
    }
}
