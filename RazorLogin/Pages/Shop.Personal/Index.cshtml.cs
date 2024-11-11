using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Security.Claims;

namespace RazorLogin.Pages.Shop.Personal
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public IList<RazorLogin.Models.Employee> Employee { get; set; } = new List<RazorLogin.Models.Employee>();

        public async Task OnGetAsync()
        {
            // Retrieve the current user's email or ID
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                // Handle missing email case
            }

            // Fetch employee records matching the logged-in user's email
            Employee = await _context.Employees
                .Where(e => e.EmployeeEmail == userEmail)
                .ToListAsync();
        }
    }
}
