using System;
using System.Collections.Generic;
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

        public IndexModel(ZooDbContext context)
        {
            _connectionString = context.Database.GetDbConnection().ConnectionString;
        }

        public IList<Event> Events { get; set; } = new List<Event>();

        public async Task OnGetAsync()
        {
            var query = "SELECT Event_ID, Event_name, Event_start_time, Event_end_time, Event_date, Event_Location FROM Event";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Events.Add(new Event
                                {
                                    EventId = reader.GetInt32(reader.GetOrdinal("Event_ID")),
                                    EventName = reader.GetString(reader.GetOrdinal("Event_name")),
                                    EventStartTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("Event_start_time"))),
                                    EventEndTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("Event_end_time"))),
                                    EventDate = reader.IsDBNull(reader.GetOrdinal("Event_date")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Event_date"))),
                                    EventLocation = reader.GetString(reader.GetOrdinal("Event_Location"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while fetching the events: {ex.Message}");
            }
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

