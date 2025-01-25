using PollForge.SharedKernel;

namespace PollForge.Domain.UserEntity;

public class User : Entity
{
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    
    private readonly List<UserSession> _sessions = [];
    public IReadOnlyCollection<UserSession> Sessions => _sessions.AsReadOnly();

    private User()
    {
    }

    public static User Create(string fullName, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email
        };
    }

    public void UpdateFullName(string fullName)
    {
        FullName = fullName;
    }
    
    public void AddSession(string sessionState, string refreshToken, DateTime createdAtUtc, DateTime expiresAtUtc)
    {
        var session = UserSession.Create(sessionState, refreshToken, createdAtUtc, expiresAtUtc);
        _sessions.Add(session);
    }

    public void RefreshSession(UserSession session, string refreshToken, DateTime expiresAtUtc)
    {
        var newSession = UserSession.Create(session.SessionState, refreshToken, session.CreatedAtUtc, expiresAtUtc);
        var indexOfSession = _sessions.IndexOf(session);
        _sessions[indexOfSession] = newSession;
    }
}
