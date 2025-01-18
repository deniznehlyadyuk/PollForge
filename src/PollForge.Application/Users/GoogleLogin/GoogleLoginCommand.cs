using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Users.GoogleLogin;

public sealed record GoogleLoginCommand(string Code, string CodeVerifier) : ICommand;
