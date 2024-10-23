using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using Microsoft.Extensions.Logging;

namespace RazorLogin.Pages.CustomerPage
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

        public Customer Customer { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch the logged-in user's email
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            // Log the email for debugging
            _logger.LogInformation($"Logged in user's email: {userEmail}");

            // Get the customer's record based on the user's email (Querying the database for customer information)
            // For example, the SQL query for this operation would be: SELECT TOP(1) FROM Customers WHERE CustomerEmail = 'UserEmail@example.com';
            // This would select the first record from the Customers table that matches the specified email.
            if (!string.IsNullOrEmpty(userEmail))
            {
                Customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);

                // Log the result of the query for debugging
                if (Customer == null)
                {
                    _logger.LogWarning($"No customer found for email: {userEmail}");
                }
                else
                {
                    _logger.LogInformation($"Customer found: {Customer.CustomerFirstName} {Customer.CustomerLastName}");
                }
            }
            else
            {
                _logger.LogWarning("User email is null or empty.");
            }
        }
    }
}
