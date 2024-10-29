

///
///  FOOD STORES
/// 

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Linq;

namespace RazorLogin.Pages.Manag.Report
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public IList<StoreReport> StoreReports { get; set; } = new List<StoreReport>();
        public int TotalStores { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        // Stores with Min and Max Average Salaries
        public StoreReport? StoreWithMinAvgSalary { get; set; }
        public StoreReport? StoreWithMaxAvgSalary { get; set; }

        public async Task OnGetAsync(int page = 1, string search = null)
        {
            CurrentPage = page;

            var query = _context.FoodStores.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(fs => fs.FoodStoreName.Contains(search) || fs.FoodStoreId.ToString() == search);
            }

            TotalStores = await query.CountAsync();

            StoreReports = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(fs => new StoreReport
                {
                    FoodStoreId = fs.FoodStoreId,
                    FoodStoreName = fs.FoodStoreName,
                    NumberOfEmployees = fs.Employees.Count(),
                    MinSalary = fs.Employees.Min(e => e.EmployeeSalary) ?? 0,
                    MaxSalary = fs.Employees.Max(e => e.EmployeeSalary) ?? 0,
                    AvgSalary = fs.Employees.Average(e => e.EmployeeSalary) ?? 0
                })
                .ToListAsync();

            if (string.IsNullOrEmpty(search))
            {
                // Calculate the store with the minimum average salary
                StoreWithMinAvgSalary = await _context.FoodStores
                    .Select(fs => new StoreReport
                    {
                        FoodStoreId = fs.FoodStoreId,
                        FoodStoreName = fs.FoodStoreName,
                        AvgSalary = fs.Employees.Any() ? fs.Employees.Average(e => e.EmployeeSalary) ?? 0 : 0
                    })
                    .OrderBy(sr => sr.AvgSalary)
                    .FirstOrDefaultAsync();

                // Calculate the store with the maximum average salary
                StoreWithMaxAvgSalary = await _context.FoodStores
                    .Select(fs => new StoreReport
                    {
                        FoodStoreId = fs.FoodStoreId,
                        FoodStoreName = fs.FoodStoreName,
                        AvgSalary = fs.Employees.Any() ? fs.Employees.Average(e => e.EmployeeSalary) ?? 0 : 0
                    })
                    .OrderByDescending(sr => sr.AvgSalary)
                    .FirstOrDefaultAsync();
            }
        }
    }

    public class StoreReport
        {
            public int FoodStoreId { get; set; }
            public string FoodStoreName { get; set; } = null!;
            public int NumberOfEmployees { get; set; }
            public int MinSalary { get; set; }
            public int MaxSalary { get; set; }
            public double AvgSalary { get; set; }
        }
    }
