using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.FShps
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
        public FoodStore FoodStore { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the highest ShopId from the database, if any
            var maxShopId = await _context.FoodStores
                                            .OrderByDescending(s => s.FoodStoreId)
                                            .Select(s => s.FoodStoreId)
                                            .FirstOrDefaultAsync();

            // Set the new ShopId by incrementing the maximum ShopId found (or default to 1 if no ShopId exists)
            FoodStore.FoodStoreId = maxShopId + 1;

            _context.FoodStores.Add(FoodStore);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
