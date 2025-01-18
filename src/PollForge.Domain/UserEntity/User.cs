using PollForge.SharedKernel;

namespace PollForge.Domain.UserEntity;

public class User : Entity
{
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    private User()
    {
    }

    public static User Create(string fullName, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
        };
    }
}
