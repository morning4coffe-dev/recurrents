using System.Text.Json.Serialization;

namespace ProjectSBS.DataContracts;

public record Currency
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [JsonPropertyName("base")]
    public string BaseCurrency { get; set; }

    [JsonPropertyName("rates")]
    public Dictionary<string, double> Rates { get; set; }
}
