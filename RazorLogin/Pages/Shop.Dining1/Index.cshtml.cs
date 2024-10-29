using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Dining1
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public List<FoodStore> FoodStores { get; set; } = new List<FoodStore>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    // SQL query to fetch all food stores
                    var foodStoreQuery = "SELECT * FROM Food_store";
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = foodStoreQuery;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var foodStores= new List<FoodStore>();
                            while (await reader.ReadAsync())
                            {
                                // Since I am having problems with mapping the TimeOnly data type, I will read them as strings instead and convert them to TimeOnly manually
                                var openTimeString = reader["Food_store_open_time"].ToString();
                                var closeTimeString = reader["Food_store_close_time"].ToString();

                                // Parsing the times manually
                                var openTime = TimeOnly.Parse(openTimeString);
                                var closeTime = TimeOnly.Parse(closeTimeString);

                                foodStores.Add(new FoodStore
                                {
                                    FoodStoreId = reader.GetInt32(reader.GetOrdinal("Food_store_ID")),
                                    FoodStoreName = reader.GetString(reader.GetOrdinal("Food_store_name")),
                                    FoodStoreLocation = reader.GetString(reader.GetOrdinal("Food_store_location")),
                                    FoodStoreOpenTime= openTime,
                                    FoodStoreCloseTime = closeTime,
                                    FoodStoreType = reader.GetString(reader.GetOrdinal("Food_store_type"))
                                });
                            }
                            FoodStores = foodStores;
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
    }
}
