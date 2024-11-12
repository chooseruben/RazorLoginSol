using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.MyShop
{
    public class DetailsModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DetailsModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }
        public FoodStore FoodStore { get; set; }
        public GiftShop GiftShop { get; set; }
        public List<RazorLogin.Models.Employee> Employee { get; set; } = new List<RazorLogin.Models.Employee>();
        public int EmployeeCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftShopQ = "SELECT * FROM Gift_shop WHERE Shop_ID = @shopId";
            var employeeQ = "SELECT * FROM Employee WHERE Department = 'GIFT' AND Shop_ID = @shopId";
            var EmployeecountQ = "SELECT COUNT(*) FROM Employee WHERE Shop_ID = @shopId";

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    // Fetch Gift Shop details
                    using (var giftShopCommand = connection.CreateCommand())
                    {
                        giftShopCommand.CommandText = giftShopQ;
                        giftShopCommand.Parameters.Add(new SqlParameter("@shopId", id));

                        using (var reader = await giftShopCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Retrieve as string or DateTime and convert to TimeOnly
                                var openTimeString = reader["Gift_shop_open_time"].ToString();
                                var closeTimeString = reader["Gift_shop_close_time"].ToString();

                                var openTime = TimeOnly.Parse(openTimeString);
                                var closeTime = TimeOnly.Parse(closeTimeString);

                                GiftShop = new GiftShop
                                {
                                    ShopId = reader.GetInt32(reader.GetOrdinal("Shop_ID")),
                                    GiftShopName = reader.GetString(reader.GetOrdinal("Gift_shop_Name")),
                                    GiftShopLocation = reader.GetString(reader.GetOrdinal("Gift_shop_Location")),
                                    GiftShopOpenTime = openTime,
                                    GiftShopCloseTime = closeTime
                                };
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }

                    // Fetch employees in this gift shop
                    using (var employeeCommand = connection.CreateCommand())
                    {
                        employeeCommand.CommandText = employeeQ;
                        employeeCommand.Parameters.Add(new SqlParameter("@shopId", id));

                        using (var reader = await employeeCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Employee.Add(new RazorLogin.Models.Employee
                                {
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("Employee_ID")),
                                    EmployeeFirstName = reader.GetString(reader.GetOrdinal("Employee_First_name")),
                                    EmployeeLastName = reader.GetString(reader.GetOrdinal("Employee_Last_name")),
                                    Department = reader.GetString(reader.GetOrdinal("Department"))
                                });
                            }
                        }
                    }

                    // Fetch employee count in this gift shop
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = EmployeecountQ;
                        command.Parameters.Add(new SqlParameter("@shopId", id));

                        EmployeeCount = (int)await command.ExecuteScalarAsync();
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
