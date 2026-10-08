using System.Runtime.CompilerServices;
using Microsoft.Extensions.DataIngestion;

namespace OracleVectorDataAiChatApp.Services.Ingestion;

/// <summary>
/// Ensures text written to Oracle's default NVARCHAR2(2000) columns stays within
/// the provider's character limit. The semantic chunker limits tokens, which is
/// not sufficient to enforce a character-based database column limit.
/// </summary>
internal sealed class OracleStringLengthChunkProcessor : IngestionChunkProcessor<string>
{
    public override async IAsyncEnumerable<IngestionChunk<string>> ProcessAsync(
        IAsyncEnumerable<IngestionChunk<string>> chunks,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var chunk in chunks.WithCancellation(cancellationToken))
        {
            var context = LimitString(chunk.Context, IngestedChunk.OracleStringMaxLength);

            foreach (var content in SplitString(chunk.Content, IngestedChunk.OracleStringMaxLength))
            {
                var limitedChunk = new IngestionChunk<string>(content, chunk.Document, context);

                if (chunk.HasMetadata)
                {
                    foreach (var metadata in chunk.Metadata)
                    {
                        limitedChunk.Metadata[metadata.Key] = metadata.Value;
                    }
                }

                yield return limitedChunk;
            }
        }
    }

    private static IEnumerable<string> SplitString(string value, int maxLength)
    {
        for (var offset = 0; offset < value.Length;)
        {
            var length = Math.Min(maxLength, value.Length - offset);
            var end = offset + length;

            // string.Length counts UTF-16 code units. Do not split a surrogate pair,
            // and remain conservative so Oracle receives no more than maxLength
            // characters/code points even for supplementary Unicode characters.
            if (end < value.Length && char.IsHighSurrogate(value[end - 1]) && char.IsLowSurrogate(value[end]))
            {
                length--;
            }

            yield return value.Substring(offset, length);
            offset += length;
        }
    }

    private static string? LimitString(string? value, int maxLength)
    {
        if (value is null || value.Length <= maxLength)
        {
            return value;
        }

        var length = maxLength;
        if (char.IsHighSurrogate(value[length - 1]) && char.IsLowSurrogate(value[length]))
        {
            length--;
        }

        return value[..length];
    }
}
