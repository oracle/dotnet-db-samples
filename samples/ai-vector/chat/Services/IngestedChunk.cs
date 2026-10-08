using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.VectorData;

namespace OracleVectorDataAiChatApp.Services;

public class IngestedChunk
{
    public const int VectorDimensions = 1536; // 1536 is the default vector size for the OpenAI text-embedding-3-small model
    public const string VectorDistanceFunction = DistanceFunction.CosineDistance;
    public const string CollectionName = "data-oraclevectordataaichatapp-chunks";
    public const int OracleStringMaxLength = 2000;

    [VectorStoreKey(StorageName = "key")]
    [JsonPropertyName("key")]
    public required Guid Key { get; set; }

    [VectorStoreData(StorageName = "documentid")]
    [MaxLength(OracleStringMaxLength)]
    [JsonPropertyName("documentid")]
    public required string DocumentId { get; set; }

    [VectorStoreData(StorageName = "content")]
    [MaxLength(OracleStringMaxLength)]
    [JsonPropertyName("content")]
    public required string Text { get; set; }

    [VectorStoreData(StorageName = "context")]
    [MaxLength(OracleStringMaxLength)]
    [JsonPropertyName("context")]
    public string? Context { get; set; }

    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction, StorageName = "embedding")]
    [JsonPropertyName("embedding")]
    public string Vector => Text;
}
