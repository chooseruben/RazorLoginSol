using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorLogin.Models;

namespace RazorLogin.Pages.AnimalsPage
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userZookeeper;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userZookeeper)
        {
            _context = context;
            _userZookeeper = userZookeeper;
        }

        public IActionResult OnGet()
        {
        ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "EnclosureId", "EnclosureId");
        ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers, "ZookeeperId", "ZookeeperId");
            return Page();
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

   
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers, "ZookeeperId", "ZookeeperId");
                ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "EnclosureId", "EnclosureId");
                return Page();
            }

            _context.Animals.Add(Animal);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
