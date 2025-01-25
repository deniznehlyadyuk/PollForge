using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Users.KeycloakLogin;

public sealed record LoginCommand(string Code, string CodeVerifier) : ICommand;
