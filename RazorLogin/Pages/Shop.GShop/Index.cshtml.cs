using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.GShop
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public List<GiftShop> GiftShops { get; set; } = new List<GiftShop>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    // SQL query to fetch all food stores
                    var GShopQ = "SELECT * FROM Gift_shop";
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = GShopQ;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var giftShops = new List<GiftShop>();
                            while (await reader.ReadAsync())
                            {
                                // Since I am having problems with mapping the TimeOnly data type, I will read them as strings instead and convert them to TimeOnly manually
                                var openTimeString = reader["Gift_shop_open_time"].ToString();
                                var closeTimeString = reader["Gift_shop_close_time"].ToString();

                                // Parsing the times manually
                                var openTime = TimeOnly.Parse(openTimeString);
                                var closeTime = TimeOnly.Parse(closeTimeString);

                                giftShops.Add(new GiftShop
                                {
                                    ShopId = reader.GetInt32(reader.GetOrdinal("Shop_ID")),
                                    GiftShopName = reader.GetString(reader.GetOrdinal("Gift_shop_Name")),
                                    GiftShopLocation = reader.GetString(reader.GetOrdinal("Gift_shop_Location")),
                                    GiftShopOpenTime = openTime,
                                    GiftShopCloseTime = closeTime,
                                });
                            }
                            GiftShops = giftShops;
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
