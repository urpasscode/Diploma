using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class SNNameRepository : DbContext
    {
        public SNNameRepository(DbContextOptions<SNNameRepository> options)
        : base(options)
        {
        }

        public DbSet<SN_Name> SN_Names { get; set; }
    }
}
