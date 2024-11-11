using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient; // Import the correct namespace
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Pur
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public async Task OnGetAsync()
        {
            // Get the connection string from configuration
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Create a new connection using the Microsoft.Data.SqlClient namespace
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Define your SQL query
                string query = "SELECT * FROM Purchase ORDER BY Customer_ID";

                // Create a command object to execute the query
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    // Read the data and map it to your model
                    while (await reader.ReadAsync())
                    {
                        var purchase = new Purchase
                        {
                            PurchaseId = reader.GetInt32(reader.GetOrdinal("Purchase_ID")),
                            CustomerId = reader.IsDBNull(reader.GetOrdinal("Customer_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Customer_ID")),
                            StoreId = reader.IsDBNull(reader.GetOrdinal("Store_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Store_ID")),
                            NumItems = reader.IsDBNull(reader.GetOrdinal("num_items")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("num_items")),
                            TotalPurchasesPrice = reader.IsDBNull(reader.GetOrdinal("Total_purchases_price")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Total_purchases_price")),
                            PurchaseMethod = reader.IsDBNull(reader.GetOrdinal("Purchase_Method")) ? null : reader.GetString(reader.GetOrdinal("Purchase_Method")),
                        };

                        Purchases.Add(purchase);
                    }
                }
            }
        }
    }
}
