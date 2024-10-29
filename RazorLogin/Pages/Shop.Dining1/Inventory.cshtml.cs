using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Dining1
{
    public class InventoryModel : PageModel
    {
        private readonly string _connectionString;
        public InventoryModel(RazorLogin.Models.ZooDbContext context)
        {
            // Fetch the connection string from the context options
            _connectionString = context.Database.GetDbConnection().ConnectionString;
        }

        public int FoodStoreId { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();

        public async Task<IActionResult> OnGetAsync(int? foodStoreId)
        {
            if (foodStoreId == null)
            {
                return NotFound(); // No shop ID provided
            }
            FoodStoreId = foodStoreId.Value;
            var inventoryQuery = "SELECT * FROM Item WHERE Food_store_ID = @foodStoreId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Fetch inventory items
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = inventoryQuery;
                        command.Parameters.Add(new SqlParameter("@foodStoreId", foodStoreId));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Fetch RestockDate as DateTime and convert to DateOnly
                                var restockDate = reader.GetDateTime(reader.GetOrdinal("Restock_date"));
                                var dateOnly = DateOnly.FromDateTime(restockDate);

                                Items.Add(new Item
                                {
                                    ItemId = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                                    ItemName = reader.GetString(reader.GetOrdinal("Item_name")),
                                    ItemCount = reader.GetInt32(reader.GetOrdinal("Item_count")),
                                    RestockDate = dateOnly,
                                    ShopId = reader.IsDBNull(reader.GetOrdinal("Shop_ID")) ? null : reader.GetInt32(reader.GetOrdinal("Shop_ID")),
                                    FoodStoreId = reader.GetInt32(reader.GetOrdinal("Food_store_ID"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) // Catch any errors that occur
            {
                ModelState.AddModelError("", $"An error occurred while fetching the inventory: {ex.Message}");
                return Page(); // Return current page with the error message
            }

            return Page(); // Return the page with the data
        }
        public async Task<IActionResult> OnPostDeleteAsync(int itemId, int foodStoreId)
        {
            var deleteQuery = "DELETE FROM Item WHERE Item_ID = @itemId AND Food_store_ID = @foodStoreId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = deleteQuery;
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));
                        command.Parameters.Add(new SqlParameter("@foodStoreId", foodStoreId));

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the item: {ex.Message}");
                return Page();
            }

            // Redirect back to the inventory page after deletion
            return RedirectToPage("./Inventory", new { foodStoreId });
        }
    }
}