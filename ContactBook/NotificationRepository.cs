using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class NotificationRepository : DbContext
    {
        public NotificationRepository(DbContextOptions<NotificationRepository> options)
        : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
    }
}
