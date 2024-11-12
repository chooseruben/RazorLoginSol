using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class AddModel : PageModel
    {
        
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Item Item { get; set; }

        [BindProperty]
        public string StoreType { get; set; }

        [BindProperty]
        public int StoreId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999);
            Item.ItemId = randomSuffix;

            while (await _context.Items.AnyAsync(d => d.ItemId == Item.ItemId))
            {
                randomSuffix = random.Next(10000, 99999);
                Item.ItemId = randomSuffix;
            }

            // Set the appropriate store ID based on the selected StoreType
            if (StoreType == "GiftShop")
            {
                Item.ShopId = StoreId;
                Item.FoodStoreId = null;
            }
            else if (StoreType == "FoodStore")
            {
                Item.ShopId = null;
                Item.FoodStoreId = StoreId;
            }
            else
            {
                ModelState.AddModelError("", "Please select a valid store type.");
                return Page();
            }

            // Insert item into the database using raw SQL to directly set ItemId
            var sqlQuery = "INSERT INTO Item (item_ID, Item_name, Item_count, Restock_date, Shop_ID, Food_store_ID) " +
                           "VALUES (@ItemId, @ItemName, @ItemCount, @RestockDate, @ShopId, @FoodStoreId)";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ItemId", Item.ItemId));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ItemName", Item.ItemName));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ItemCount", Item.ItemCount));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@RestockDate", Item.RestockDate));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ShopId", Item.ShopId ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FoodStoreId", Item.FoodStoreId ?? (object)DBNull.Value));
                    await command.ExecuteNonQueryAsync();
                }
            }

            return Item.ShopId.HasValue
                ? RedirectToPage("./Inventory", new { shopId = Item.ShopId })
                : RedirectToPage("./Inventory", new { foodStoreId = Item.FoodStoreId });
        
        }
    }
}
