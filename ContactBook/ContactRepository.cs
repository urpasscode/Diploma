using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class ContactRepository : DbContext
    {
        public ContactRepository(DbContextOptions<ContactRepository> options)
        : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
    }
}
