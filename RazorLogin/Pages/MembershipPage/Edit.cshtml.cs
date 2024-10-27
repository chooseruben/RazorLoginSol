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

        public IEnumerable<SelectListItem> MembershipOptions { get; set; }

        public async Task OnGetAsync(int id)
        {
            // Load the customer data from the database
            Customer = await _context.Customers.FindAsync(id);

            // Membership options
            MembershipOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Free", Text = "Free" },
                new SelectListItem { Value = "Family", Text = "Family" },
                new SelectListItem { Value = "VIP", Text = "VIP" }
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update the customer's membership details in the database
            var customerToUpdate = await _context.Customers.FindAsync(Customer.CustomerId);

            if (customerToUpdate != null)
            {
                customerToUpdate.MembershipType = Customer.MembershipType;
                customerToUpdate.MembershipStartDate = DateOnly.FromDateTime(DateTime.Now); // Update to current date
                customerToUpdate.MembershipEndDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)); // Update to one year from now

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
