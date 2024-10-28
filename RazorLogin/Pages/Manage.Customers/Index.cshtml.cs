using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manage.Customers
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<RazorLogin.Models.Employee> Employee { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Employee = await _context.Employees
                .Include(e => e.FoodStore)
                .Include(e => e.Shop)
                .Include(e => e.Supervisor).ToListAsync();
        }
    }
}
