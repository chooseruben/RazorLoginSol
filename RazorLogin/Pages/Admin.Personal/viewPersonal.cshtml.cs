using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Personal
{
    [Authorize(Roles = "Admin")]
    public class viewPersonalModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public viewPersonalModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if the id is valid
            if (id <= 0)
            {
                return NotFound();
            }

            Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Return to the same page to show validation errors
            }

            // Update the Employee entity
            var employeeToUpdate = await _context.Employees.FindAsync(Employee.EmployeeId);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // Check if the email has changed
            if (employeeToUpdate.EmployeeEmail != Employee.EmployeeEmail)
            {
                // Update the identity user email
                var identityUser = await _userManager.FindByEmailAsync(employeeToUpdate.EmployeeEmail);
                if (identityUser != null)
                {
                    identityUser.Email = Employee.EmployeeEmail;
                    var result = await _userManager.UpdateAsync(identityUser);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page(); // Return to show validation errors
                    }
                }
            }

            // Update the employee record
            employeeToUpdate.EmployeeEmail = Employee.EmployeeEmail;
            employeeToUpdate.EmployeeFirstName = Employee.EmployeeFirstName; // Update other properties as needed
            employeeToUpdate.EmployeeLastName = Employee.EmployeeLastName; // Update other properties as needed
            // Continue updating other properties...

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
