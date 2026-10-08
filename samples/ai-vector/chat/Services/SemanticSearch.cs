using OracleVectorDataAiChatApp.Services.Ingestion;
using Microsoft.Extensions.VectorData;

namespace OracleVectorDataAiChatApp.Services;

public class SemanticSearch(
    VectorStoreCollection<Guid, IngestedChunk> vectorCollection,
    [FromKeyedServices("ingestion_directory")] DirectoryInfo ingestionDirectory,
    DataIngestor dataIngestor)
{
    private Task? _ingestionTask;
    private readonly object _ingestionLock = new();

    public Task LoadDocumentsAsync()
    {
        lock (_ingestionLock)
        {
            // Share one attempt across users. A later request can retry a failed
            // attempt; successful attempts remain cached for this process.
            if (_ingestionTask is null || _ingestionTask.IsFaulted || _ingestionTask.IsCanceled)
            {
                _ingestionTask = dataIngestor.IngestDataAsync(ingestionDirectory, searchPattern: "*.*");
            }

            return _ingestionTask;
        }
    }

    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        // Ensure documents have been loaded before searching
        await LoadDocumentsAsync();

        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });

        return await nearest.Select(result => result.Record).ToListAsync();
    }
}
