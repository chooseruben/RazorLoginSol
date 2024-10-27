using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Reports2
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // Properties for user input
        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public bool SearchAll { get; set; } = true; // Default to search all employees

        public ReportData Report { get; set; }

        public async Task OnGetAsync()
        {
            Report = new ReportData();

            var query = _context.Set<Employee>().AsQueryable();

            // Filter by date if StartDate and EndDate are provided
            if (StartDate.HasValue && EndDate.HasValue)
            {
                query = query.Where(e => e.DateOfEmployment.HasValue
                    && e.DateOfEmployment.Value.ToDateTime(TimeOnly.MinValue) >= StartDate.Value
                    && e.DateOfEmployment.Value.ToDateTime(TimeOnly.MinValue) <= EndDate.Value);
            }

            var employees = await query.ToListAsync(); // Retrieve data into memory

            // 1. Employee Stats
            var employeeStats = employees
                .GroupBy(e => e.Degree)
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Average(e => e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    MinAge = g.Min(e => e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    MaxAge = g.Max(e => e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    Count = g.Count()
                })
                .ToList();

            Report.EmployeeStats = employeeStats;

            // 2. Manager Stats
            var managerStats = await _context.Set<Manager>()
                .Join(_context.Set<Employee>(), m => m.EmployeeId, e => e.EmployeeId, (m, e) => new { e, m })
                .ToListAsync(); // Retrieve data into memory

            var managerGroupedStats = managerStats
                .GroupBy(em => em.e.Degree)
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Average(em => em.e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - em.e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    MinAge = g.Min(em => em.e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - em.e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    MaxAge = g.Max(em => em.e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - em.e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    Count = g.Count()
                })
                .ToList();

            Report.ManagerStats = managerGroupedStats;

            // 3. Zookeeper Stats
            var zookeeperStats = await _context.Set<Zookeeper>()
                .Join(_context.Set<Employee>(), z => z.EmployeeId, e => e.EmployeeId, (z, e) => new { e, z })
                .ToListAsync(); // Retrieve data into memory

            var zookeeperGroupedStats = zookeeperStats
                .GroupBy(ze => ze.e.Degree)
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Average(ze => ze.e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - ze.e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    MinAge = g.Min(ze => ze.e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - ze.e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    MaxAge = g.Max(ze => ze.e.EmployeeDob.HasValue
                        ? DateTime.Now.Year - ze.e.EmployeeDob.Value.Year - (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)
                        : 0),
                    Count = g.Count()
                })
                .ToList();

            Report.ZookeeperStats = zookeeperGroupedStats;

            // Prepare data for the bar chart if necessary
        }
    }

    public class ReportData
    {
        public List<StatDto> EmployeeStats { get; set; } = new List<StatDto>();
        public List<StatDto> ManagerStats { get; set; } = new List<StatDto>();
        public List<StatDto> ZookeeperStats { get; set; } = new List<StatDto>();
    }

    public class StatDto
    {
        public string Degree { get; set; }
        public double AverageAge { get; set; }
        public double MinAge { get; set; }
        public double MaxAge { get; set; }
        public int Count { get; set; }
    }
}
