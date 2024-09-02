using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class TagRepository : DbContext
    {
        public TagRepository(DbContextOptions<TagRepository> options)
        : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }
    }
}
