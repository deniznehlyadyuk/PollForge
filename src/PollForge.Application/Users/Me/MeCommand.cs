using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Users.Me;

public sealed record MeCommandResponse(string Email, string FullName);

public sealed record MeCommand() : ICommand<MeCommandResponse>;