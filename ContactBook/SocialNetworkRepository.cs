using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class SocialNetworkRepository : DbContext
    {
        public SocialNetworkRepository(DbContextOptions<SocialNetworkRepository> options)
        : base(options)
        {
        }

        public DbSet<SocialNetwork> SocialNetworks { get; set; }
    }
}
