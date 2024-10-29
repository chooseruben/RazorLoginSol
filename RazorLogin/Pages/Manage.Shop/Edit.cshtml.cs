using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manage.Shop
{
    [Authorize(Roles = "Manager")]
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GiftShop GiftShop { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }

            GiftShop = await _context.GiftShops.FirstOrDefaultAsync(m => m.ShopId == id);
    
            if (GiftShop == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var giftShopToUpdate = await _context.GiftShops.FindAsync(GiftShop.ShopId);
            if (giftShopToUpdate == null)
            {
                return NotFound();
            }

            giftShopToUpdate.GiftShopName = GiftShop.GiftShopName;
            giftShopToUpdate.GiftShopOpenTime = GiftShop.GiftShopOpenTime;
            giftShopToUpdate.GiftShopCloseTime = GiftShop.GiftShopCloseTime;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

    }
}
