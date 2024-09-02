using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class EventRepository : DbContext
    {
        public EventRepository(DbContextOptions<EventRepository> options)
        : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
    }
}
