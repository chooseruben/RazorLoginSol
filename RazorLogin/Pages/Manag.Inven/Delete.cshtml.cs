using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class DeleteModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Item Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToPage("/Account/Login");

            Item = await _context.Items.FindAsync(id);
            if (Item == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUser.Email);
            if (employee?.ShopId != Item.ShopId && employee?.FoodStoreId != Item.FoodStoreId)
                return Unauthorized();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return item.ShopId.HasValue
                ? RedirectToPage("./Inventory", new { shopId = item.ShopId })
                : RedirectToPage("./Inventory", new { foodStoreId = item.FoodStoreId });
        }
    }
}
