using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Eve
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        { 
            _context = context;
            _connectionString = context.Database.GetDbConnection().ConnectionString;
        }


        public IList<Event> Events { get; set; }

            public async Task OnGetAsync()
            {
                Events = await _context.Events.ToListAsync();
            }

        public async Task<IActionResult> OnPostDeleteAsync(int eventId)
        {
            var deleteQuery = "DELETE FROM Event WHERE Event_ID = @eventId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = deleteQuery;
                        command.Parameters.Add(new SqlParameter("@eventId", eventId));

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the event: {ex.Message}");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
﻿



