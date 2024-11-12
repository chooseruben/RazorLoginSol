using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class EditStoreModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditStoreModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public GiftShop AssignedGiftShop { get; set; }

        [BindProperty]
        public FoodStore AssignedFoodStore { get; set; }

        public async Task<IActionResult> OnGetAsync(int storeId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToPage("/Account/Login");

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUser.Email);

            // Load store details based on the employee's assignment
            if (employee?.ShopId == storeId)
            {
                AssignedGiftShop = await _context.GiftShops.FindAsync(storeId);
            }
            else if (employee?.FoodStoreId == storeId)
            {
                AssignedFoodStore = await _context.FoodStores.FindAsync(storeId);
            }
            else
            {
                return Unauthorized();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (AssignedGiftShop != null)
            {
                var store = await _context.GiftShops.FindAsync();
                if (store != null)
                {
                    store.GiftShopName = AssignedGiftShop.GiftShopName;
                    store.GiftShopLocation = AssignedGiftShop.GiftShopLocation;
                    store.GiftShopOpenTime = AssignedGiftShop.GiftShopOpenTime;
                    store.GiftShopCloseTime = AssignedGiftShop.GiftShopCloseTime;

                    _context.Attach(store).State = EntityState.Modified;
                }
            }
            else if (AssignedFoodStore != null)
            {
                var store = await _context.FoodStores.FindAsync();
                if (store != null)
                {
                    store.FoodStoreName = AssignedFoodStore.FoodStoreName;
                    store.FoodStoreLocation = AssignedFoodStore.FoodStoreLocation;
                    store.FoodStoreOpenTime = AssignedFoodStore.FoodStoreOpenTime;
                    store.FoodStoreCloseTime = AssignedFoodStore.FoodStoreCloseTime;

                    _context.Attach(store).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
