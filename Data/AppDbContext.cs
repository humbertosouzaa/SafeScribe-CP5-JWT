using Microsoft.EntityFrameworkCore;
using SafeScribe.Api.Models;

namespace SafeScribe.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Note> Notes => Set<Note>();
    }
}
