using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1
{
    public class EmailRepository : DbContext
    {
        public EmailRepository(DbContextOptions<EmailRepository> options)
        : base(options)
        {
        }

        public DbSet<Email> Emails { get; set; }
    }
}
