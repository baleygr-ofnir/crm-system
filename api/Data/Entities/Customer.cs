using Newtonsoft.Json;

namespace api.Data.Entities;

public record Customer
{
    [JsonProperty("id")]
    public required string Id { get; init; }

    [JsonProperty("name")]
    public required string Name { get; init; }
    
    [JsonProperty("title")]
    public required string Title { get; init; }
    
    [JsonProperty("email")]
    public required string Email { get; init; }
    
    [JsonProperty("phone")]
    public required string Phone { get; init; }
    
    [JsonProperty("address")]
    public required string Address { get; init; }
    
    [JsonProperty("salesRep")]
    public required SalesRep SalesRep { get; init; }
}