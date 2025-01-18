using PollForge.Domain.UserEntity;

namespace PollForge.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}
