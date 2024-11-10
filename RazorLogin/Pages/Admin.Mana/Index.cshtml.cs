using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<ManagerViewModel> Managers { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Load managers with their employee and store data (FoodStore and GiftShop)
            var managers = await _context.Managers
                .Include(m => m.Employee)
                .ThenInclude(e => e.FoodStore) // Include the FoodStore related to the employee
                .Include(m => m.Employee)
                .ThenInclude(e => e.Shop) // Include the GiftShop related to the employee
                .ToListAsync();

            // Map the managers to a view model with the assigned store names
            Managers = managers.Select(m => new ManagerViewModel
            {
                ManagerId = m.ManagerId,
                EmployeeId = m.EmployeeId,
                Department = m.Department,
                ManagerEmploymentDate = m.ManagerEmploymentDate,
                AssignedStore = GetStoreName(m.Employee), // Get the store name for each manager's employee
                EmployeeFirstName = m.Employee.EmployeeFirstName ?? "N/A", // Get the employee's first name
                EmployeeLastName = m.Employee.EmployeeLastName ?? "N/A"  // Get the employee's last name
            }).ToList();
        }

        // Helper method to get the store name (either FoodStore or GiftShop)
        private string GetStoreName(Employee employee)
        {
            if (employee.FoodStore != null)
            {
                return employee.FoodStore.FoodStoreName; // FoodStore is assigned
            }
            else if (employee.Shop != null)
            {
                return employee.Shop.GiftShopName; // GiftShop is assigned
            }
            else
            {
                return "No store assigned"; // No store assigned
            }
        }
    }

    // ViewModel to display manager details with assigned store
    public class ManagerViewModel
    {
        public int ManagerId { get; set; }
        public int EmployeeId { get; set; }
        public string Department { get; set; }
        public DateOnly? ManagerEmploymentDate { get; set; }
        public string AssignedStore { get; set; } // Store name (FoodStore or GiftShop)

        // Add these properties to hold the employee's name
        public string EmployeeFirstName { get; set; } = null!;
        public string EmployeeLastName { get; set; } = null!;
    }
}




/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<RazorLogin.Models.Manager> Manager { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Manager = await _context.Managers
                .Include(m => m.Employee).ToListAsync();
        }
    }
}*/
