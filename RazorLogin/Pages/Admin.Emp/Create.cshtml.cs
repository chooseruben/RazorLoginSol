using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

   

        public IActionResult OnGet()
        {
        ViewData["FoodStoreId"] = new SelectList(_context.FoodStores, "FoodStoreId", "FoodStoreId");

            List<SelectListItem> selectList = _context.FoodStores
                .AsNoTracking()
                .Select(x => new SelectListItem()
                {
                    Value = x.FoodStoreId.ToString(),
                  
                })
                .ToList();

            selectList.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = "--- Select Related Entity ---"
            });

            ViewData["RelatedEntity_Id"] = selectList;

            ViewData["ShopId"] = new SelectList(_context.GiftShops, "ShopId", "ShopId");
            ViewData["SupervisorId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            var user = new IdentityUser
            {
                UserName = Employee.EmployeeEmail,
                Email = Employee.EmployeeEmail
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (!result.Succeeded)
            {
                // Handle errors (e.g., display error messages)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            if (!string.IsNullOrEmpty(Role))
            {
                await _userManager.AddToRoleAsync(user, Role);
            }


            return RedirectToPage("./Index");
        }
    }


}
