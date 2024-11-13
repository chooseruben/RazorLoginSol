using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerShop
{
    public class MyPurchasesModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MyPurchasesModel> _logger;

        public MyPurchasesModel(RazorLogin.Models.ZooDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<MyPurchasesModel> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public class PurchaseViewModel
        {
            public int PurchaseId { get; set; }
            public int NumItems { get; set; }
            public int? TotalPurchasesPrice { get; set; }
            public DateOnly? PurchaseDate { get; set; }
            public TimeOnly? PurchaseTime { get; set; }
            public string ItemName { get; set; } // New property for item name
        }

        public IList<PurchaseViewModel> Purchases { get; set; } = new List<PurchaseViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the current user's email
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login");
            }

            // Find the customer associated with the logged-in user's email
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Fetch purchases associated with the customer's CustomerId and exclude those with NumItems == 0,
            // Order by PurchaseDate and PurchaseTime in descending order to show the most recent purchase first
            var purchases = await _context.Purchases
                .Where(p => p.CustomerId == customer.CustomerId && (p.NumItems ?? 0) > 0)
                .OrderByDescending(p => p.PurchaseDate)
                .ThenByDescending(p => p.PurchaseTime)
                .ToListAsync();

            // Map the purchases to the view model
            Purchases = purchases.Select(p => new PurchaseViewModel
            {
                PurchaseId = p.PurchaseId,
                NumItems = p.NumItems ?? 0,
                TotalPurchasesPrice = p.TotalPurchasesPrice,
                PurchaseDate = p.PurchaseDate,
                PurchaseTime = p.PurchaseTime,
                ItemName = (p.NumItems > 1)
                    ? "Bundle"  // Always say "Bundle" for quantity > 1
                    : (p.TotalPurchasesPrice switch
                    {
                        12 => "Cup",
                        30 => "Hat",
                        25 => "Cap",
                        40 => "Shirt",
                        50 => "Pants",
                        _ => "Unknown"
                    })
            }).ToList();

            return Page();
        }
    }
}
