//ParticipantService/Options/RetryOptions.cs

namespace ParticipantService.Options;

public class RetryOptions
{
    public int MaxRetries { get; set; }
    public int InitialDelaySeconds { get; set; }
}