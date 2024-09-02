using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class ElementTagRepository : DbContext
    {
        public ElementTagRepository(DbContextOptions<ElementTagRepository> options)
        : base(options)
        {
        }

        public DbSet<Element_Tag> Elements_Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Element_Tag>()
                  .HasKey(m => new { m.tag_id, m.elem_id });
        }
    }
}
