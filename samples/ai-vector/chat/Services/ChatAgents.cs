using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace OracleVectorDataAiChatApp.Services;

public static class ChatAgentPrompts
{
    public const string System = @"
        You are an assistant who answers questions about information you retrieve.
        Do not answer questions about anything else.
        Use only simple markdown to format your responses.

        Use the LoadDocuments tool to prepare for searches before answering any questions.

        Use the Search tool to find relevant information. When you do this, end your
        reply with citations in the special XML format:

        <citation filename='string'>exact quote here</citation>

        Always include the citation in your response if there are results.

        The quote must be max 5 words, taken word-for-word from the search result, and is the basis for why the citation is relevant.
        Don't refer to the presence of citations; just emit these tags right at the end, with no surrounding text.
        ";

    public const string Suggestions = @"
        Suggest up to 3 follow-up questions that I could ask you to help me complete my task.
        Each suggestion must be a complete sentence, maximum 6 words.
        Each suggestion must be phrased as something that I (the user) would ask you (the assistant) in response to your previous message,
        for example 'How do I do that?' or 'Explain ...'.
        If there are no suggestions, reply with an empty list.
        ";
}

public sealed class ChatAgentTools(SemanticSearch search)
{
    [Description("Loads the documents needed for performing searches. Must be completed before a search can be executed, but only needs to be completed once.")]
    public Task LoadDocumentsAsync() => search.LoadDocumentsAsync();

    [Description("Searches for information using a phrase or keyword. Relies on documents already being loaded.")]
    public async Task<IEnumerable<string>> SearchAsync(
        [Description("The phrase to search for.")] string searchPhrase,
        [Description("If possible, specify the filename to search that file only. If not provided or empty, the search includes all files.")] string? filenameFilter = null)
    {
        var results = await search.SearchAsync(searchPhrase, filenameFilter, maxResults: 5);
        return results.Select(result =>
            $"<result filename=\"{result.DocumentId}\">{result.Text}</result>");
    }
}

public sealed class ChatAgents
{
    public ChatAgents(IChatClient chatClient, ChatAgentTools tools, ILoggerFactory loggerFactory)
    {
        Main = new ChatClientAgent(
            chatClient,
            instructions: ChatAgentPrompts.System,
            name: "CustomDataAssistant",
            description: "Answers questions using the application's indexed documents.",
            tools:
            [
                AIFunctionFactory.Create(tools.LoadDocumentsAsync),
                AIFunctionFactory.Create(tools.SearchAsync)
            ],
            loggerFactory: loggerFactory);

        Suggestions = new ChatClientAgent(
            chatClient,
            instructions: ChatAgentPrompts.Suggestions,
            name: "FollowUpSuggestionAssistant",
            description: "Suggests concise follow-up questions for the current conversation.",
            loggerFactory: loggerFactory);
    }

    public AIAgent Main { get; }

    public AIAgent Suggestions { get; }
}
