using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Authentication.Me;

public sealed record MeCommandResponse(string Email, string FullName);

public sealed record MeCommand() : ICommand<MeCommandResponse>;