using Microsoft.EntityFrameworkCore;
using PollForge.Domain.UserEntity;

namespace PollForge.Application.Abstractions.Data;

public interface IPollForgeDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
