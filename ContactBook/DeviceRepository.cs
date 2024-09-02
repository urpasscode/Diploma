using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class DeviceRepository : DbContext
    {
        public DeviceRepository(DbContextOptions<DeviceRepository> options)
        : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
    }
}
