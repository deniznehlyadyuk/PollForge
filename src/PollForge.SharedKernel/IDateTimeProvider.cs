namespace PollForge.SharedKernel;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
