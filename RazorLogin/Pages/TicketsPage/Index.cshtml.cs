using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorLogin.Models;

namespace RazorLogin.Pages.TicketsPage
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(RazorLogin.Models.ZooDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<IndexModel> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IList<Ticket> Tickets { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch the logged-in user's email
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            // Log the email for debugging
            _logger.LogInformation($"Logged in user's email: {userEmail}");

            // Initialize the list of tickets
            Tickets = new List<Ticket>();

            // Get the customer's record based on the user's email
            if (!string.IsNullOrEmpty(userEmail))
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);

                // Log the result of the query for debugging
                if (customer == null)
                {
                    _logger.LogWarning($"No customer found for email: {userEmail}");
                }
                else
                {
                    _logger.LogInformation($"Customer found: {customer.CustomerFirstName} {customer.CustomerLastName}");

                    // Fetch tickets for the found customer
                    Tickets = await _context.Tickets
                        .Include(t => t.Purchase) 
                        .Where(t => t.Purchase.CustomerId == customer.CustomerId)
                        .ToListAsync();
                }
            }
            else
            {
                _logger.LogWarning("User email is null or empty.");
            }
        }
    }
}
