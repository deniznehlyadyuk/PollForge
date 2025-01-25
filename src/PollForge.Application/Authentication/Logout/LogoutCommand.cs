using PollForge.Application.Abstractions.Messaging;

namespace PollForge.Application.Authentication.Logout;

public sealed record LogoutCommand() : ICommand;