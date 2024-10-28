using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AspNetUser AspNetUser { get; set; } = default!;

        [BindProperty]
        public Employee Employee { get; set; } = default!; //new Employee(); // Initialize it


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(Employee Employee)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AspNetUsers.Add(AspNetUser);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
