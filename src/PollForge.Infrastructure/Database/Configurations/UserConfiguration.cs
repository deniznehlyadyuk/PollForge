using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PollForge.Domain.UserEntity;

namespace PollForge.Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.Property(user => user.FullName).IsRequired();
        builder.Property(user => user.Email).IsRequired();
        
        builder.OwnsMany(user => user.Sessions, session =>
        {
            session.WithOwner().HasForeignKey("UserId");
            session.Property(userSession => userSession.RefreshToken).IsRequired();
            session.Property(userSession => userSession.CreatedAtUtc).IsRequired();
            session.Property(userSession => userSession.ExpiresAtUtc).IsRequired();
        });
    }
}