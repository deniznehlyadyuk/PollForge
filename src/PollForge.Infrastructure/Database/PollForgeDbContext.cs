using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using PollForge.Domain.UserEntity;

namespace PollForge.Infrastructure.Database;

public class PollForgeDbContext : DbContext, IPollForgeDbContext
{
    public PollForgeDbContext(DbContextOptions<PollForgeDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
