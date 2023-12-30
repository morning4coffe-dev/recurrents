namespace ProjectSBS.Business.Models;

public record AppConfig
{
    public string? Environment { get; init; }

    public const string TelemetryId = "";

    public const string ApplicationId = "";
    public const string TenantId = "";
    public static string[] Scopes =
    [

    ];
}
