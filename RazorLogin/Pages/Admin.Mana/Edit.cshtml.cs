using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RazorLogin.Models.Manager Manager { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager =  await _context.Managers.FirstOrDefaultAsync(m => m.ManagerId == id);
            if (manager == null)
            {
                return NotFound();
            }
            Manager = manager;
           ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
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

            _context.Attach(Manager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(Manager.ManagerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ManagerExists(int id)
        {
            return _context.Managers.Any(e => e.ManagerId == id);
        }
    }
}
