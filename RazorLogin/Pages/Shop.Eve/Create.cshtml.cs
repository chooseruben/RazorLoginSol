using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Security.Claims;

namespace RazorLogin.Pages.Shop.Eve
{
    public class CreateModel : PageModel
    {
        private readonly ZooDbContext _context;

        public CreateModel(ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // No need for dropdown setup since we automatically set EmployeeRepId on post
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a unique EventId
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999);
            Event.EventId = randomSuffix;

            while (await _context.Events.AnyAsync(d => d.EventId == Event.EventId))
            {
                randomSuffix = random.Next(10000, 99999);
                Event.EventId = randomSuffix;
            }

            // Get the logged-in user's ID and set it as the EmployeeRepId
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee != null)
            {
                Event.EventEmployeeRepId = employee.EmployeeId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to identify the current user as an employee.");
                return Page();
            }

            try
            {
                _context.Events.Add(Event);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("An event is already scheduled at this date, time, and location."))
                {
                    ModelState.AddModelError(string.Empty, "This event cannot be scheduled because another event is already scheduled at the same date, time, and location.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving your data. Please try again.");
                }

                return Page();
            }
        }
    }
}
