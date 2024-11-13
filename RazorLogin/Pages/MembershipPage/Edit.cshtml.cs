using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task OnGetAsync(int id)
        {
            // Load the customer data from the database
            Customer = await _context.Customers.FindAsync(id);

            if (Customer == null)
            {
                // Handle case where customer is not found, redirect or show error
                TempData["ErrorMessage"] = "Customer not found.";
                return;
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
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "There was an error with your submission.";
                return Page();
            }

            // Update the customer's membership details in the database
            var customerToUpdate = await _context.Customers.FindAsync(Customer.CustomerId);

            if (customerToUpdate == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToPage("./Index");
            }

            customerToUpdate.MembershipType = Customer.MembershipType;
            customerToUpdate.MembershipStartDate = DateOnly.FromDateTime(DateTime.Now); // Set to current date
            customerToUpdate.MembershipEndDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)); // Set to one year from now

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your membership details have been updated successfully!";

            MembershipOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "FREE TIER", Text = "FREE TIER" },
            new SelectListItem { Value = "FAMILY TIER", Text = "FAMILY TIER" },
            new SelectListItem { Value = "VIP TIER", Text = "VIP TIER" }
        };

            var selectedMembership = MembershipOptions.FirstOrDefault(x => x.Value == Customer.MembershipType);
            if (selectedMembership != null)
            {
                selectedMembership.Selected = true;
            }

            return Page();
        }
    }

}