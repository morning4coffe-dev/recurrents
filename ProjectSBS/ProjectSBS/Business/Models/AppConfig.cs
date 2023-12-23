namespace ProjectSBS.Business.Models;

public record AppConfig
{
    public string? Environment { get; init; }
    public const string TelemetryId = "ea15cf3d-e346-483c-b0b3-618fa515fd50";
}
