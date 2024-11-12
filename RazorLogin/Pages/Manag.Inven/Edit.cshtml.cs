using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class EditModel : PageModel
    {
        
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Item Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserEmail = currentUser?.Email;

            if (string.IsNullOrEmpty(currentUserEmail))
                return RedirectToPage("/Account/Login");

            // Find the item and ensure it's within the manager's assigned store
            Item = await _context.Items.FirstOrDefaultAsync(m => m.ItemId == id);
            if (Item == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUserEmail);

            if (employee?.ShopId != Item.ShopId && employee?.FoodStoreId != Item.FoodStoreId)
                return Unauthorized();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Item.ShopId.HasValue
                ? RedirectToPage("./Inventory", new { shopId = Item.ShopId })
                : RedirectToPage("./Inventory", new { foodStoreId = Item.FoodStoreId });
        }
    }
}
