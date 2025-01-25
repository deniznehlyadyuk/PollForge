using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Authentication.Login;

public sealed record LoginCommand(string Code, string CodeVerifier) : ICommand;
