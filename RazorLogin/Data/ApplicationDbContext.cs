using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RazorLogin.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        internal readonly IEnumerable<object> Customer;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
