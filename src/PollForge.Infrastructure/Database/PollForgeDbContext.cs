using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PollForge.Domain.UserEntity;

namespace PollForge.Infrastructure.Database;

public class PollForgeDbContext : IdentityDbContext<User>
{
    public PollForgeDbContext(DbContextOptions<PollForgeDbContext> options) : base(options)
    {
    }
}
