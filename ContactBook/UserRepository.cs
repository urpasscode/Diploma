using WebApplication1.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication1
{
    public class UserRepository : DbContext
    {
        public UserRepository(DbContextOptions<UserRepository> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
