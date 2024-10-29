using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.GShop.Dining1
{
    public class AddItemModel : PageModel
    {

        [BindProperty]
        public Item Item { get; set; } = new Item();
        public int FoodStoreId {  get; set; }

        public readonly RazorLogin.Models.ZooDbContext _context;
        public AddItemModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Item.FoodStoreId = FoodStoreId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Insert item into the database using raw SQL
            var sqlQuery = "INSERT INTO item(item_ID, Item_name, Item_count, Restock_date, Food_store_ID) VALUES (@ItemId, @ItemName, @ItemCount, @RestockDate, @FoodStoreId)";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    
                }
            }

            // Redirect to the inventory page based on which ID (shopId or foodStoreId) was used
            if (Item.ShopId.HasValue)
            {
                return RedirectToPage("./Inventory", new { shopId = Item.ShopId });
            }
            else if (Item.FoodStoreId.HasValue)
            {
                return RedirectToPage("./Inventory", new { FoodStoreId = Item.FoodStoreId });
            }
            else
            {
                ModelState.AddModelError("", "Unable to determine the correct inventory page to redirect to.");
                return Page();
            }
        }
    }
}
