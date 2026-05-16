namespace SolarMetrics.Web.Services;

public sealed record TokenAcquisitionResult(string? Token, string? FailureMessage)
{
    public static TokenAcquisitionResult Success(string token) => new(token, null);

    public static TokenAcquisitionResult Failed(string message) => new(null, message);
}
