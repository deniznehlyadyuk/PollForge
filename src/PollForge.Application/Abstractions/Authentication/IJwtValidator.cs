namespace PollForge.Application.Abstractions.Authentication;

public interface IJwtValidator
{
    Task<bool> ValidateTokenAsync(string token);
}