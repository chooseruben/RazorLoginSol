using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;

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

        public SelectList EmployeeRepOptions { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch employees for the dropdown list
            var employees = await _context.Employees
                .Select(e => new { e.EmployeeId, FullName = e.EmployeeFirstName + " " + e.EmployeeLastName })
                .ToListAsync();

            EmployeeRepOptions = new SelectList(employees, "EmployeeId", "FullName");
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
                    // General error message for other database issues
                    ModelState.AddModelError(string.Empty, "An error occurred while saving your data. Please try again.");
                }

                return Page();
            }
        }
    }
}
