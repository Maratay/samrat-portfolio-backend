using Microsoft.EntityFrameworkCore;
using ContactApi.Models;

namespace ContactApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ContactMessage> ContactMessages { get; set; }
}