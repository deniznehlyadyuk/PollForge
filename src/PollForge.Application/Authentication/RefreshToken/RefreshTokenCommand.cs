using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand() : ICommand;