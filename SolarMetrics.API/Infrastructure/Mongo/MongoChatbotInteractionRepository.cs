using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SolarMetrics.Configuration;

namespace SolarMetrics.Infrastructure.Mongo;

public sealed class MongoChatbotInteractionRepository : IChatbotInteractionRepository
{
    private readonly IMongoCollection<ChatbotInteractionDocument> _collection;

    public bool IsEnabled => true;

    public MongoChatbotInteractionRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;
        var databaseName = string.IsNullOrWhiteSpace(settings.DatabaseName) ? "solarmetrics" : settings.DatabaseName;
        var collectionName = string.IsNullOrWhiteSpace(settings.ChatbotInteractionsCollection)
            ? "chatbot_interactions"
            : settings.ChatbotInteractionsCollection;

        _collection = mongoClient.GetDatabase(databaseName).GetCollection<ChatbotInteractionDocument>(collectionName);
    }

    public async Task<(IReadOnlyList<ChatbotInteractionDocument> Items, long TotalCount)> ListRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var filter = Builders<ChatbotInteractionDocument>.Filter.Empty;
        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _collection
            .Find(filter)
            .SortByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var indexKeys = Builders<ChatbotInteractionDocument>.IndexKeys.Descending(x => x.CreatedAtUtc);
        var indexModel = new CreateIndexModel<ChatbotInteractionDocument>(indexKeys);
        return _collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
    }
}
