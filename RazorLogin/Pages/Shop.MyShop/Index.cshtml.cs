using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Shop.MyShop
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public FoodStore FoodStore { get; set; }
        public GiftShop GiftShop { get; set; }

        public async Task OnGetAsync()
        {
            // Retrieve the current user's email or ID
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                // Handle missing email case
                return;
            }

            // Fetch employee record for the logged-in user
            var employee = await _context.Employees
                .Include(e => e.FoodStore) // Include related FoodStore
                .Include(e => e.Shop)  // Include related GiftShop
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee != null)
            {
                FoodStore = employee.FoodStore;
                GiftShop = employee.Shop;
            }
        }
    }
}
