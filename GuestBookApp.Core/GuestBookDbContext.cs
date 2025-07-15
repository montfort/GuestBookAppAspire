using Microsoft.EntityFrameworkCore;
using GuestBookApp.Core.Models;

namespace GuestBookApp.Core.Data
{
    public class GuestBookDbContext : DbContext
    {
        public GuestBookDbContext(DbContextOptions<GuestBookDbContext> options) : base(options) { }

        public DbSet<GuestBookEntry> GuestBookEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sembrado de datos iniciales (ver sección de sembrado de datos)
            //...
        }
    }
}