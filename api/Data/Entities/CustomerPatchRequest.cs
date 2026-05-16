using Newtonsoft.Json;

namespace api.Data.Entities;

public record CustomerPatchRequest
{
    [JsonProperty("name")]
    public string? Name { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("email")]
    public string? Email { get; init; }

    [JsonProperty("phone")]
    public string? Phone { get; init; }

    [JsonProperty("address")]
    public string? Address { get; init; }

    [JsonProperty("salesRep")]
    public SalesRep? SalesRep { get; init; }
}