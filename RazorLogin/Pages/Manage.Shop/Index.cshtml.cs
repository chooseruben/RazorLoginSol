using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manage.Shop
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;


        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<GiftShop> GiftShops { get; set; } = new List<GiftShop>();

        public async Task OnGetAsync()
        {
                GiftShops = await _context.GiftShops.ToListAsync();
        }
    }
}
