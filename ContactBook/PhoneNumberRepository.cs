using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class PhoneNumberRepository : DbContext
    {
        public PhoneNumberRepository(DbContextOptions<PhoneNumberRepository> options)
        : base(options)
        {
        }

        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    }
}
