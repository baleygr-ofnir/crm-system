using Newtonsoft.Json;

namespace api.Data.Entities;

public record SalesRep
{
    [JsonProperty("name")]
    public required string Name { get; init; }

    [JsonProperty("normalizedName")]
    public string NormalizedName { get; init; }

    [JsonProperty("email")]
    public required string Email { get; init; }
    
    [JsonProperty("phone")]
    public required string Phone { get; init; }
}