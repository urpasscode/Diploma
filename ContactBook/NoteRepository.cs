using WebApplication1.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication1
{
    public class NoteRepository : DbContext
    {
        public NoteRepository(DbContextOptions<NoteRepository> options)
        : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
    }
}
