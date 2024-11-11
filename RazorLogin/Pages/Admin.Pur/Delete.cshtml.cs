using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Pur
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Purchase Purchase { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get the connection string from configuration
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Query to get the purchase record by PurchaseId
                string query = "SELECT * FROM Purchase WHERE Purchase_ID = @PurchaseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PurchaseId", id);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        Purchase = new Purchase
                        {
                            PurchaseId = reader.GetInt32(reader.GetOrdinal("Purchase_ID")),
                            CustomerId = reader.IsDBNull(reader.GetOrdinal("Customer_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Customer_ID")),
                            StoreId = reader.IsDBNull(reader.GetOrdinal("Store_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Store_ID")),
                            NumItems = reader.IsDBNull(reader.GetOrdinal("num_items")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("num_items")),
                            TotalPurchasesPrice = reader.IsDBNull(reader.GetOrdinal("Total_purchases_price")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Total_purchases_price")),
                            PurchaseMethod = reader.IsDBNull(reader.GetOrdinal("Purchase_Method")) ? null : reader.GetString(reader.GetOrdinal("Purchase_Method")),
                        };
                    }
                }
            }

            if (Purchase == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Get the connection string from configuration
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Query to delete the purchase record
                string query = "DELETE FROM Purchase WHERE Purchase_ID = @PurchaseId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PurchaseId", id);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToPage("Index"); // Redirect back to the list page
        }
    }
}
