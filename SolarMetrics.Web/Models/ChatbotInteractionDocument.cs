using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SolarMetrics.Web.Models;

public sealed class ChatbotInteractionDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }

    [BsonElement("userId")]
    public string? UserId { get; set; }

    [BsonElement("userName")]
    public string? UserName { get; set; }

    [BsonElement("question")]
    public string Question { get; set; } = string.Empty;

    [BsonElement("answer")]
    public string? Answer { get; set; }

    [BsonElement("source")]
    public string? Source { get; set; }

    [BsonElement("success")]
    public bool Success { get; set; }

    [BsonElement("errorMessage")]
    public string? ErrorMessage { get; set; }

    [BsonElement("durationMs")]
    public long DurationMs { get; set; }

    [BsonElement("clientIp")]
    public string? ClientIp { get; set; }

    [BsonElement("userAgent")]
    public string? UserAgent { get; set; }
}
