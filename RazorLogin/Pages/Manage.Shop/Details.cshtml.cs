﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manage.Shop
{
    public class DetailsModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DetailsModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

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
    }
}
