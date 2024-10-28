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
                .GroupBy(e => e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(e => e.EmployeeDob.HasValue)
                                  .Average(e => DateTime.Now.Year - e.EmployeeDob.Value.Year -
                                                (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(e => e.EmployeeDob.HasValue)
                               .Min(e => DateTime.Now.Year - e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(e => e.EmployeeDob.HasValue)
                               .Max(e => DateTime.Now.Year - e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            // If no stats found, set a message
            if (!employeeStats.Any())
            {
                employeeStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.EmployeeStats = employeeStats;

            // 2. Manager Stats
            var managerStats = await _context.Set<Manager>()
                .Join(_context.Set<Employee>(), m => m.EmployeeId, e => e.EmployeeId, (m, e) => new { e, m })
                .ToListAsync(); // Retrieve data into memory

            var managerGroupedStats = managerStats
                .GroupBy(em => em.e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(em => em.e.EmployeeDob.HasValue)
                                  .Average(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                                 (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(em => em.e.EmployeeDob.HasValue)
                               .Min(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(em => em.e.EmployeeDob.HasValue)
                               .Max(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            if (!managerGroupedStats.Any())
            {
                managerGroupedStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.ManagerStats = managerGroupedStats;

            // 3. Zookeeper Stats
            var zookeeperStats = await _context.Set<Zookeeper>()
                .Join(_context.Set<Employee>(), z => z.EmployeeId, e => e.EmployeeId, (z, e) => new { e, z })
                .ToListAsync(); // Retrieve data into memory

            var zookeeperGroupedStats = zookeeperStats
                .GroupBy(ze => ze.e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                                  .Average(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                                 (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                               .Min(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                               .Max(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            if (!zookeeperGroupedStats.Any())
            {
                zookeeperGroupedStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.ZookeeperStats = zookeeperGroupedStats;

            // Prepare data for average ages overview
            Report.AverageAgesOverview = new double[]
            {
                employeeStats.Average(s => s.AverageAge),
                managerGroupedStats.Average(s => s.AverageAge),
                zookeeperGroupedStats.Average(s => s.AverageAge)
            };

            // Prepare data for degree counts overview
            Report.DegreeCountsOverview = new Dictionary<string, int>();
            foreach (var stat in employeeStats.Concat(managerGroupedStats).Concat(zookeeperGroupedStats))
            {
                if (!string.IsNullOrEmpty(stat.Degree)) // Check if Degree is not null or empty
                {
                    if (Report.DegreeCountsOverview.ContainsKey(stat.Degree))
                    {
                        Report.DegreeCountsOverview[stat.Degree] += stat.Count;
                    }
                    else
                    {
                        Report.DegreeCountsOverview[stat.Degree] = stat.Count;
                    }
                }
            }
        }
    }

    public class ReportData
    {
        public List<StatDto> EmployeeStats { get; set; } = new List<StatDto>();
        public List<StatDto> ManagerStats { get; set; } = new List<StatDto>();
        public List<StatDto> ZookeeperStats { get; set; } = new List<StatDto>();

        // New properties for graphs
        public double[] AverageAgesOverview { get; set; }
        public Dictionary<string, int> DegreeCountsOverview { get; set; } = new Dictionary<string, int>();
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
