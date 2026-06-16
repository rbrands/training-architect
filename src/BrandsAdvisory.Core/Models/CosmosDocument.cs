using System.Text.Json.Serialization;

namespace BrandsAdvisory.Core.Models;

/// <summary>
/// Abstract base class for all Cosmos DB documents.
/// The Type property doubles as the partition key value.
/// </summary>
public abstract class CosmosDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("type")]
    public abstract string Type { get; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("ttl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TimeToLive { get; set; }
}
