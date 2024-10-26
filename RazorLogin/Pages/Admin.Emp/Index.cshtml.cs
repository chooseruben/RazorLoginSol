using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Employee> Employee { get;set; } = default!;
        public IList<AspNetRole> Role { get; set; } = default!;

        public Dictionary<string, IList<string>> EmployeeRoles { get; set; } = new Dictionary<string, IList<string>>();


        public async Task OnGetAsync()
        {
            Employee = await _context.Employees
                .Include(e => e.FoodStore)
                .Include(e => e.Shop)
                .Include(e => e.Supervisor).ToListAsync();

            foreach (var employee in Employee)
            {
                if (!string.IsNullOrEmpty(employee.EmployeeEmail)) 
                {

                    var user = await _userManager.FindByEmailAsync(employee.EmployeeEmail);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        EmployeeRoles[employee.EmployeeEmail] = roles.ToList();
                    }

                   // var roles = await _userManager.GetRolesAsync(new IdentityUser { Id = employee.EmployeeEmail });
                    //EmployeeRoles[employee.EmployeeEmail] = roles.ToList();
                }
            }
        }
    }
}
