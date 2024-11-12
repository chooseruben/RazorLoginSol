using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Zook.Eve
{
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        // This method retrieves the event to be edited by its ID
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the event and related employee rep
            Event = await _context.Events
                .Include(e => e.EventEmployeeRep) // Include the employee representative (if needed)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (Event == null)
            {
                return NotFound();
            }

            return Page();
        }

        // This method updates the event and saves changes to the database
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventToUpdate = await _context.Events.FindAsync(id);

            if (eventToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Event>(
                eventToUpdate,
                "Event",   // Prefix for the binded properties
                e => e.EventName, e => e.EventStartTime, e => e.EventEndTime,
                e => e.EventLocation, e => e.EventDate, e => e.EventEmployeeRepId))
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page(); // In case of validation errors, return the same page with error messages
        }
    }
}
