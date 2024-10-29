using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Dining1
{
    public class EditingModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditingModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int itemId, int foodStoreId)
        {
            var query = "SELECT * FROM Item WHERE Item_ID = @itemId AND Food_store_ID = @foodStoreId";

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));
                        command.Parameters.Add(new SqlParameter("@foodStoreId", foodStoreId));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Item = new Item
                                {
                                    ItemId = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                                    ItemName = reader.GetString(reader.GetOrdinal("Item_name")),
                                    ItemCount = reader.GetInt32(reader.GetOrdinal("Item_count")),
                                    RestockDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Restock_date"))),
                                    ShopId = reader.IsDBNull(reader.GetOrdinal("Shop_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Shop_ID")),
                                    FoodStoreId = reader.GetInt32(reader.GetOrdinal("Food_store_ID"))
                                };
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while fetching data: {ex.Message}");
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var query = "UPDATE Item SET Item_count = @itemCount, Restock_date = @restockDate WHERE Item_ID = @itemId AND Food_store_ID = @foodStoreId";

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@itemCount", Item.ItemCount));
                        command.Parameters.Add(new SqlParameter("@restockDate", Item.RestockDate.ToDateTime(TimeOnly.MinValue)));
                        command.Parameters.Add(new SqlParameter("@itemId", Item.ItemId));
                        command.Parameters.Add(new SqlParameter("@foodStoreId", Item.FoodStoreId));

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving data: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Inventory", new { foodStoreId = Item.FoodStoreId });
        }
    }
}