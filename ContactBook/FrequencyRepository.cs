using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class FrequencyRepository : DbContext
    {
        public FrequencyRepository(DbContextOptions<FrequencyRepository> options)
        : base(options)
        {
        }

        public DbSet<Frequency> Frequencies { get; set; }
    }
}
