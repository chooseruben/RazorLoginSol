using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
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

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
