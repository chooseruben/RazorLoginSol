using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorLogin.Models;

namespace RazorLogin.Pages.TicketsPage
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        // Ticket Types and Prices
        public Dictionary<string, int> TicketTypes { get; } = new Dictionary<string, int>
        {
            { "Adult", 25 },
            { "Child", 10 },
            { "Senior", 10 },
            { "Veteran", 7 }
        };

        // Properties for dropdowns
        public List<string> AvailableEntryTimes { get; set; } = new List<string>();

        // Selected values from dropdowns
        [BindProperty]
        public string SelectedTicketType { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedEntryTime { get; set; } = string.Empty;

        public void OnGet()
        {
            AvailableEntryTimes = GetAvailableEntryTimes();
        }

        private List<string> GetAvailableEntryTimes()
        {
            // Example entry times
            return new List<string> { "10:00 AM", "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM", "4:00 PM", "5:00 PM", "6:00 PM" };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }

            // Assign selected ticket type to the Ticket model
            Ticket.TicketType = SelectedTicketType;

            // Set the ticket price based on selected ticket type
            if (TicketTypes.TryGetValue(SelectedTicketType, out var price))
            {
                Ticket.TicketPrice = price;
            }
            else
            {
                ModelState.AddModelError("Ticket.TicketType", "Invalid ticket type selected.");
                return Page();
            }

            // Generate PurchaseId and set the ticket purchase date
            Ticket.PurchaseId = GeneratePurchaseId();
            Ticket.TicketPurchaseDate = DateOnly.FromDateTime(DateTime.Now);

            // Set the ticket entry time
            if (TimeOnly.TryParse(SelectedEntryTime, out var entryTime))
            {
                Ticket.TicketEntryTime = entryTime;
            }
            else
            {
                ModelState.AddModelError("Ticket.TicketEntryTime", "Invalid entry time selected.");
                return Page();
            }

            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();

            return RedirectToPage("/TicketsPage/Index");
        }

        private int GeneratePurchaseId()
        {
            return new Random().Next(1, 10000); // Placeholder implementation
        }
    }
}
