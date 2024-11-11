using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public IActionResult OnGet()
        {

            // Get a list of Gift Shops that do NOT have managers assigned to them
            var GiftShopsWithoutManagers = _context.GiftShops
                .Where(shop => !_context.Managers
                    .Any(mgr => mgr.EmployeeId != null &&
                                _context.Employees
                                    .Where(emp => emp.EmployeeId == mgr.EmployeeId)
                                    .Any(emp => emp.ShopId == shop.ShopId))) // No manager assigned to this shop
                .Select(shop => new SelectListItem
                {
                    Value = shop.ShopId.ToString(),
                    Text = shop.GiftShopName // Display the shop name (or any other relevant field)
                })
                .ToList();


            // Get a list of Food Stores that do NOT have managers assigned to them
            var FoodStoresWithoutManagers = _context.FoodStores
                .Where(store => !_context.Managers
                    .Any(mgr => mgr.EmployeeId != null &&
                                _context.Employees
                                    .Where(emp => emp.EmployeeId == mgr.EmployeeId)
                                    .Any(emp => emp.FoodStoreId == store.FoodStoreId))) // No manager assigned to this food store
                .Select(store => new SelectListItem
                {
                    Value = store.FoodStoreId.ToString(),
                    Text = store.FoodStoreName // Display the store name (or any other relevant field)
                })
                .ToList();


            ///////////////////////////////////////////////////////////////////////////////////////

            // Get a list of Gift Shops that have managers assigned to them
            var giftShopsWithManagers = _context.Managers
                .Where(mgr => mgr.EmployeeId != null)  // Make sure the manager has an employee linked
                .Join(_context.Employees, // Join the Manager table with the Employee table
                    mgr => mgr.EmployeeId, // Using EmployeeId as the key
                    emp => emp.EmployeeId,
                    (mgr, emp) => new { emp.ShopId }) // Project the ShopId from Employee table
                .Where(x => x.ShopId.HasValue) // Only include records where ShopId is not null
                .Select(x => x.ShopId.Value)  // Select the ShopId to filter the stores
                .Distinct() // Get distinct ShopIds (to avoid duplicate stores)
                .ToList();

            // Get a list of Food Stores that have managers assigned to them
            var foodStoresWithManagers = _context.Managers
                .Where(mgr => mgr.EmployeeId != null)  // Make sure the manager has an employee linked
                .Join(_context.Employees, // Join the Manager table with the Employee table
                    mgr => mgr.EmployeeId, // Using EmployeeId as the key
                    emp => emp.EmployeeId,
                    (mgr, emp) => new { emp.FoodStoreId }) // Project the FoodStoreId from Employee table
                .Where(x => x.FoodStoreId.HasValue) // Only include records where FoodStoreId is not null
                .Select(x => x.FoodStoreId.Value)  // Select the FoodStoreId to filter the stores
                .Distinct() // Get distinct FoodStoreIds (to avoid duplicate stores)
                .ToList();

            // Now, fetch the Gift Shops that have managers assigned
            var GiftShopsWithManagers = _context.GiftShops
                .Where(shop => giftShopsWithManagers.Contains(shop.ShopId)) // Only those that have managers
                .Select(shop => new SelectListItem
                {
                    Value = shop.ShopId.ToString(),
                    Text = shop.GiftShopName // You can use other fields for display as well
                })
                .ToList();

            // Now, fetch the Food Stores that have managers assigned
            var FoodStoresWithManagers = _context.FoodStores
                .Where(store => foodStoresWithManagers.Contains(store.FoodStoreId)) // Only those that have managers
                .Select(store => new SelectListItem
                {
                    Value = store.FoodStoreId.ToString(),
                    Text = store.FoodStoreName // You can use other fields for display as well
                })
                .ToList();


            // Add default options for both Gift Shops and Food Stores
            GiftShopsWithManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR EMPLOYEES ---" });
            FoodStoresWithManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR EMPLOYEES ---" });

            GiftShopsWithoutManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR MANAGERS ---" });
            FoodStoresWithoutManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR MANAGERS ---" });

            // Pass the filtered stores to the view
            ViewData["GiftShopsWithManagers"] = GiftShopsWithManagers;
            ViewData["FoodStoresWithManagers"] = FoodStoresWithManagers;


            ViewData["GiftShopsWithoutManagers"] = GiftShopsWithoutManagers;
            ViewData["FoodStoresWithoutManagers"] = FoodStoresWithoutManagers;



            // Populate supervisor selection (you may already have this set correctly)
            List<SelectListItem> selectList = _context.FoodStores
                .AsNoTracking()
                .Select(x => new SelectListItem()
                {
                    Value = x.FoodStoreId.ToString(),
                })
                .ToList();

            selectList.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = "--- Select Related Entity ---"
            });

            ViewData["RelatedEntity_Id"] = selectList;

            // You can populate ViewData for SupervisorId (Manager)
            ViewData["SupervisorId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");

            return Page();


            /* ViewData["FoodStoreId"] = new SelectList(_context.FoodStores, "FoodStoreId", "FoodStoreId");

                List<SelectListItem> selectList = _context.FoodStores
                    .AsNoTracking()
                    .Select(x => new SelectListItem()
                    {
                        Value = x.FoodStoreId.ToString(),

                    })
                    .ToList();

                selectList.Insert(0, new SelectListItem()
                {
                    Value = "",
                    Text = "--- Select Related Entity ---"
                });

                ViewData["RelatedEntity_Id"] = selectList;

                ViewData["ShopId"] = new SelectList(_context.GiftShops, "ShopId", "ShopId");
                ViewData["SupervisorId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
                return Page(); */

        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set default department to "GENERAL" if not specified
            // if (string.IsNullOrWhiteSpace(Employee.Department))
            // {
            //     Employee.Department = "GENERAL";
            // }

            // Determine department based on the role
            if (Role == "ZOOKEEPER")
            {
                Employee.Department = "ZOO";
            }
            else if (Role == "ADMIN")
            {
                Employee.Department = "GENERAL";
            }
            else if (Role == "SHOP")
            {
                // Determine department based on store assignment
                if (!string.IsNullOrEmpty(Employee.ShopId.ToString()) && Employee.ShopId != 0)
                {
                    // Employee is assigned to a Gift Shop
                    Employee.Department = "GIFT";

                    // Set the supervisor (manager) for the Gift Shop
                    var managerForGiftShop = _context.Managers
                        .FirstOrDefault(mgr => mgr.EmployeeId != null && mgr.Employee.ShopId == Employee.ShopId);
                    if (managerForGiftShop != null)
                    {
                        Employee.SupervisorId = managerForGiftShop.ManagerId;
                    }
                }
                else if (!string.IsNullOrEmpty(Employee.FoodStoreId.ToString()) && Employee.FoodStoreId != 0)
                {
                    // Employee is assigned to a Food Store
                    Employee.Department = "FOOD";

                    // Set the supervisor (manager) for the Food Store
                    var managerForFoodStore = _context.Managers
                        .FirstOrDefault(mgr => mgr.EmployeeId != null && mgr.Employee.FoodStoreId == Employee.FoodStoreId);
                    if (managerForFoodStore != null)
                    {
                        Employee.SupervisorId = managerForFoodStore.ManagerId;
                    }
                }
            }
            else if (Role == "MANAGER")
            {
                // Determine department based on store assignment
                if (!string.IsNullOrEmpty(Employee.ShopId.ToString()) && Employee.ShopId != 0)
                {
                    // Employee is assigned to a Gift Shop
                    Employee.Department = "GIFT";
                }
                else if (!string.IsNullOrEmpty(Employee.FoodStoreId.ToString()) && Employee.FoodStoreId != 0)
                {
                    // Employee is assigned to a Food Store
                    Employee.Department = "FOOD";
                }
            }
            else { Employee.Department = "GENERAL"; }


            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            var user = new IdentityUser
            {
                UserName = Employee.EmployeeEmail,
                Email = Employee.EmployeeEmail
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (!result.Succeeded)
            {
                // Handle errors (e.g., display error messages)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            if (!string.IsNullOrEmpty(Role))
            {
                await _userManager.AddToRoleAsync(user, Role);

                // Generate a random numeric suffix
                var randomSuffix = new Random().Next(10000, 99999); // Random number between 10000 and 99999

                if (Role == "MANAGER")
                {
                    // Create a new Manager entry
                    var manager = new Manager
                    {
                        EmployeeId = Employee.EmployeeId,
                        // Concatenate EmployeeId with a random suffix
                        ManagerId = int.Parse($"{Employee.EmployeeId}{randomSuffix}"), // Adjusted to be an int
                        Department = Employee.Department, // Assuming you want to set the department
                        ManagerEmploymentDate = DateOnly.FromDateTime(DateTime.Now) // Set to today's date
                    };
                    _context.Managers.Add(manager);
                    //await _context.SaveChangesAsync();
                }
                else if (Role == "ZOOKEEPER")
                {
                    // Create a new Zookeeper entry
                    var zookeeper = new Zookeeper
                    {

                        EmployeeId = Employee.EmployeeId,
                        // Concatenate EmployeeId with a random suffix
                        ZookeeperId = int.Parse($"{Employee.EmployeeId}{randomSuffix}"), // Adjusted to be an int
                        TrainingRenewalDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)) // Set to one year from now
                                                                                              // LastTrainingDate is left empty
                    };
                    _context.Zookeepers.Add(zookeeper);
                    //await _context.SaveChangesAsync(); 
                }

                // Save the new Manager or Zookeeper entry
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}