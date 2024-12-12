namespace HRManagerService.Options;

public class RetryOptions
{
    public int MaxRetries { get; set; }
    public int InitialDelaySeconds { get; set; }
}