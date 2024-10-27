using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Reports
{
    public class Index1Model : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public Index1Model(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public ReportData Report { get; set; }

        public async Task OnGetAsync()
        {
            Report = new ReportData();

            // Initialize totalEmployeeSalary
            decimal totalEmployeeSalary = 0;

            // Get avg/min/max ages and salaries for employees
            var employeeStats = await _context.Employees
                .GroupBy(e => 1)
                .Select(g => new
                {
                    AverageAge = g.Average(e => e.EmployeeDob.HasValue ? DateTime.Now.Year - e.EmployeeDob.Value.Year : 0),
                    MinAge = g.Min(e => e.EmployeeDob.HasValue ? DateTime.Now.Year - e.EmployeeDob.Value.Year : 0),
                    MaxAge = g.Max(e => e.EmployeeDob.HasValue ? DateTime.Now.Year - e.EmployeeDob.Value.Year : 0),
                    AverageSalary = g.Average(e => e.EmployeeSalary),
                    MinSalary = g.Min(e => e.EmployeeSalary),
                    MaxSalary = g.Max(e => e.EmployeeSalary),
                    TotalSalary = g.Sum(e => e.EmployeeSalary) // Calculate total salary
                })
                .FirstOrDefaultAsync();

            if (employeeStats != null)
            {
                Report.EmployeeAvgAge = employeeStats.AverageAge;
                Report.EmployeeMinAge = employeeStats.MinAge;
                Report.EmployeeMaxAge = employeeStats.MaxAge;
                Report.EmployeeAvgSalary = (decimal)(employeeStats.AverageSalary ?? 0);
                Report.EmployeeMinSalary = (decimal)(employeeStats.MinSalary ?? 0);
                Report.EmployeeMaxSalary = (decimal)(employeeStats.MaxSalary ?? 0);

                // Store total employee salary for later use
                totalEmployeeSalary = (decimal)(employeeStats.TotalSalary ?? 0);
            }

            // Yearly income from dependents
            var dependentIncomeStats = await _context.Dependants
                .GroupBy(d => d.HealthcareTier)
                .Select(g => new
                {
                    Tier = g.Key,
                    TierIncome = g.Sum(d =>
                        d.HealthcareTier == "Tier 1" ? 70 * 12 :
                        d.HealthcareTier == "Tier 2" ? 140 * 12 :
                        d.HealthcareTier == "Tier 3" ? 200 * 12 : 0)
                })
                .ToListAsync();

            Report.Tier1Income = 0;
            Report.Tier2Income = 0;
            Report.Tier3Income = 0;

            // Assign incomes to respective tiers
            foreach (var income in dependentIncomeStats)
            {
                switch (income.Tier)
                {
                    case "Tier 1":
                        Report.Tier1Income += income.TierIncome;
                        break;
                    case "Tier 2":
                        Report.Tier2Income += income.TierIncome;
                        break;
                    case "Tier 3":
                        Report.Tier3Income += income.TierIncome;
                        break;
                }
            }

            // Calculate total healthcare income
            decimal totalHealthcareIncome = Report.Tier1Income + Report.Tier2Income + Report.Tier3Income;

            // Subtract total healthcare income from total employee salaries
            decimal difference = totalEmployeeSalary - totalHealthcareIncome;

            // Store this difference in the Report
            Report.TotalDifference = difference; // Assuming this property exists in ReportData

            // Manager stats
            var managerStats = await _context.Managers
                .Join(_context.Employees,
                      m => m.EmployeeId,
                      e => e.EmployeeId,
                      (m, e) => new { e })
                .GroupBy(x => 1)
                .Select(g => new
                {
                    AverageAge = g.Average(x => x.e.EmployeeDob.HasValue ? DateTime.Now.Year - x.e.EmployeeDob.Value.Year : 0),
                    MinAge = g.Min(x => x.e.EmployeeDob.HasValue ? DateTime.Now.Year - x.e.EmployeeDob.Value.Year : 0),
                    MaxAge = g.Max(x => x.e.EmployeeDob.HasValue ? DateTime.Now.Year - x.e.EmployeeDob.Value.Year : 0),
                    AverageSalary = g.Average(x => x.e.EmployeeSalary),
                    MinSalary = g.Min(x => x.e.EmployeeSalary),
                    MaxSalary = g.Max(x => x.e.EmployeeSalary)
                })
                .FirstOrDefaultAsync();

            if (managerStats != null)
            {
                Report.ManagerAvgAge = managerStats.AverageAge;
                Report.ManagerMinAge = managerStats.MinAge;
                Report.ManagerMaxAge = managerStats.MaxAge;
                Report.ManagerAvgSalary = (decimal)(managerStats.AverageSalary ?? 0);
                Report.ManagerMinSalary = (decimal)(managerStats.MinSalary ?? 0);
                Report.ManagerMaxSalary = (decimal)(managerStats.MaxSalary ?? 0);
            }

            // Zookeeper stats
            var zookeeperStats = await _context.Zookeepers
                .Join(_context.Employees,
                      z => z.EmployeeId,
                      e => e.EmployeeId,
                      (z, e) => new { e })
                .GroupBy(x => 1)
                .Select(g => new
                {
                    AverageAge = g.Average(x => x.e.EmployeeDob.HasValue ? DateTime.Now.Year - x.e.EmployeeDob.Value.Year : 0),
                    MinAge = g.Min(x => x.e.EmployeeDob.HasValue ? DateTime.Now.Year - x.e.EmployeeDob.Value.Year : 0),
                    MaxAge = g.Max(x => x.e.EmployeeDob.HasValue ? DateTime.Now.Year - x.e.EmployeeDob.Value.Year : 0),
                    AverageSalary = g.Average(x => x.e.EmployeeSalary),
                    MinSalary = g.Min(x => x.e.EmployeeSalary),
                    MaxSalary = g.Max(x => x.e.EmployeeSalary)
                })
                .FirstOrDefaultAsync();

            if (zookeeperStats != null)
            {
                Report.ZookeeperAvgAge = zookeeperStats.AverageAge;
                Report.ZookeeperMinAge = zookeeperStats.MinAge;
                Report.ZookeeperMaxAge = zookeeperStats.MaxAge;
                Report.ZookeeperAvgSalary = (decimal)zookeeperStats.AverageSalary;
                Report.ZookeeperMinSalary = (decimal)zookeeperStats.MinSalary;
                Report.ZookeeperMaxSalary = (decimal)zookeeperStats.MaxSalary;
            }
        }
    }

    public class ReportData
    {
        public decimal TotalDifference { get; set; }
        public double EmployeeAvgAge { get; set; }
        public double EmployeeMinAge { get; set; }
        public double EmployeeMaxAge { get; set; }
        public decimal EmployeeAvgSalary { get; set; }
        public decimal EmployeeMinSalary { get; set; }
        public decimal EmployeeMaxSalary { get; set; }
        public decimal Tier1Income { get; set; }
        public decimal Tier2Income { get; set; }
        public decimal Tier3Income { get; set; }
        public double ManagerAvgAge { get; set; }
        public double ManagerMinAge { get; set; }
        public double ManagerMaxAge { get; set; }
        public decimal ManagerAvgSalary { get; set; }
        public decimal ManagerMinSalary { get; set; }
        public decimal ManagerMaxSalary { get; set; }
        public double ZookeeperAvgAge { get; set; }
        public double ZookeeperMinAge { get; set; }
        public double ZookeeperMaxAge { get; set; }
        public decimal ZookeeperAvgSalary { get; set; }
        public decimal ZookeeperMinSalary { get; set; }
        public decimal ZookeeperMaxSalary { get; set; }
    }
}
