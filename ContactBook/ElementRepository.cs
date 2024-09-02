using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class ElementRepository : DbContext
    {
        public ElementRepository(DbContextOptions<ElementRepository> options)
        : base(options)
        {
        }

        public DbSet<Element> Elements { get; set; }
    }
}
