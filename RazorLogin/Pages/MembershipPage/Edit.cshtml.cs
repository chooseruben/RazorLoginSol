using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorLogin.Pages.MembershipPage
{
    public class EditModel : PageModel
    {
        private readonly ZooDbContext _context;

        public EditModel(ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; }

        public IEnumerable<SelectListItem> MembershipOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Load the customer data from the database
            Customer = await _context.Customers.FindAsync(id);

            if (Customer == null)
            {
                // Handle case where customer is not found, redirect or show error
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToPage("./Index");
            }

            // Populate membership options
            MembershipOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "FREE TIER", Text = "FREE TIER" },
                new SelectListItem { Value = "FAMILY TIER", Text = "FAMILY TIER" },
                new SelectListItem { Value = "VIP TIER", Text = "VIP TIER" }
            };

            // Set the selected membership type
            var selectedMembership = MembershipOptions.FirstOrDefault(x => x.Value == Customer.MembershipType);
            if (selectedMembership != null)
            {
                selectedMembership.Selected = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "There was an error with your submission.";
                return Page();
            }

            // Update the customer's membership details in the database
            var customerToUpdate = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == Customer.CustomerEmail);

            if (customerToUpdate == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToPage("./Index");
            }

            // Check if membership details have changed
            bool membershipChanged = customerToUpdate.MembershipType != Customer.MembershipType ||
                                     customerToUpdate.MembershipStartDate != Customer.MembershipStartDate ||
                                     customerToUpdate.MembershipEndDate != Customer.MembershipEndDate;

            if (!membershipChanged)
            {
                // No changes in membership, no need to charge again
                TempData["SuccessMessage"] = "Your membership details have not changed, no charge applied.";
                return RedirectToPage("./Index");
            }

            // Update the customer's membership details
            customerToUpdate.MembershipType = Customer.MembershipType;
            customerToUpdate.MembershipStartDate = Customer.MembershipStartDate;
            customerToUpdate.MembershipEndDate = Customer.MembershipEndDate;

            // Generate a random PurchaseId
            var randomPurchaseId = new Random().Next(1000, 9999); // Random 4-digit number

            // Create the initial purchase record (for the first month)
            var purchase = new Purchase
            {
                PurchaseId = randomPurchaseId,  
                CustomerId = customerToUpdate.CustomerId,
                PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
                PurchaseTime = TimeOnly.FromDateTime(DateTime.Now),
                NumItems = 1,  
                ItemName = customerToUpdate.MembershipType, // Membership type as ItemName
                TotalPurchasesPrice = (int?)GetMembershipPrice(Customer.MembershipType), 
                StoreId = 2  
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Create recurring monthly charges (simulating monthly payment)
            var currentDate = DateTime.Now;
            var numMonths = 12; 

            for (int i = 1; i <= numMonths; i++)
            {
                var nextMonthDate = currentDate.AddMonths(i);

                // Create the recurring purchase for each month
                var recurringPurchase = new Purchase
                {
                    PurchaseId = new Random().Next(1000, 9999), // New random ID for each monthly charge
                    CustomerId = customerToUpdate.CustomerId,
                    PurchaseDate = DateOnly.FromDateTime(nextMonthDate),
                    PurchaseTime = TimeOnly.FromDateTime(nextMonthDate),
                    NumItems = 1,
                    ItemName = customerToUpdate.MembershipType,
                    TotalPurchasesPrice = (int?)GetMembershipPrice(Customer.MembershipType), // Monthly charge
                    StoreId = 2 
                };

                _context.Purchases.Add(recurringPurchase);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your membership details have been updated and the first charge has been applied.";

            // Repopulate membership options
            MembershipOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "FREE TIER", Text = "FREE TIER" },
        new SelectListItem { Value = "FAMILY TIER", Text = "FAMILY TIER" },
        new SelectListItem { Value = "VIP TIER", Text = "VIP TIER" }
    };

            // Set the selected membership type
            var selectedMembership = MembershipOptions.FirstOrDefault(x => x.Value == Customer.MembershipType);
            if (selectedMembership != null)
            {
                selectedMembership.Selected = true;
            }

            return Page();
        }

        private decimal GetMembershipPrice(string membershipType)
        {
            switch (membershipType)
            {
                case "FREE TIER":
                    return 0;
                case "FAMILY TIER":
                    return 15; 
                case "VIP TIER":
                    return 25; 
                default:
                    return 0;
            }
        }
    }
}

