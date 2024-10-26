using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 
using RazorLogin.Models;

namespace RazorLogin.Pages.TicketsPage
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        public Dictionary<string, int> TicketTypes { get; } = new Dictionary<string, int>
        {
            { "Adult", 25 },
            { "Child", 10 },
            { "Senior", 10 },
            { "Veteran", 7 }
        };

        [BindProperty]
        public string SelectedTicketType { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the logged-in user's email
            var user = await _userManager.GetUserAsync(User);
            var userEmail = user?.Email;

            if (userEmail == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Retrieve the customer based on the logged-in user's email
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Customer not found.");
                return Page();
            }

            // Assign selected ticket type to the Ticket model
            Ticket.TicketType = SelectedTicketType;

            // Set the ticket price based on the selected ticket type
            if (TicketTypes.TryGetValue(SelectedTicketType, out var price))
            {
                Ticket.TicketPrice = price;
            }
            else
            {
                ModelState.AddModelError("Ticket.TicketType", "Invalid ticket type selected.");
                return Page();
            }

            // Generate a unique Ticket_ID manually
            Ticket.TicketId = GenerateUniqueTicketId();

            // Create a new Purchase with a unique PurchaseId and associate it with the customer
            var purchase = new Purchase
            {
                PurchaseId = GenerateUniquePurchaseId(),
                CustomerId = customer.CustomerId 
            };

            
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Link the Purchase to the Ticket
            Ticket.Purchase = purchase;

            
            _context.Tickets.Add(Ticket);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving ticket: {ex.Message}");
                ModelState.AddModelError("Ticket", "An error occurred while saving the ticket. Please try again.");
                return Page();
            }

            return RedirectToPage("/Ticketspage/Index");
        }

        private int GenerateUniqueTicketId()
        {
            var maxTicketId = _context.Tickets.Any() ? _context.Tickets.Max(t => t.TicketId) : 0;
            return maxTicketId + 1;
        }

        private int GenerateUniquePurchaseId()
        {
            var maxId = _context.Purchases.Any() ? _context.Purchases.Max(p => p.PurchaseId) : 0;
            return maxId + 1;
        }
    }
}
