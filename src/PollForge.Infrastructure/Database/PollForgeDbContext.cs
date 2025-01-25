using Microsoft.EntityFrameworkCore;
using PollForge.Application.Abstractions.Data;
using PollForge.Domain.UserEntity;

namespace PollForge.Infrastructure.Database;

public class PollForgeDbContext(DbContextOptions<PollForgeDbContext> options) : DbContext(options), IPollForgeDbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PollForgeDbContext).Assembly);
    }
}
