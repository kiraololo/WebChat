using Microsoft.EntityFrameworkCore;
using WebChatData.Models;

namespace WebChatDataData.Models.Context
{
    public class AppIdentityDBContext : DbContext
    {
        public AppIdentityDBContext(DbContextOptions<AppIdentityDBContext> options) : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}
