using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using Oracle.ManagedDataAccess.Client;
using OracleVectorDataAiChatApp.Components;
using OracleVectorDataAiChatApp.Services;
using OracleVectorDataAiChatApp.Services.Ingestion;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Set the endpoint and key to your OpenAI account's values in appsettings.json.
// Alternatively, they can be set in Visual Studio's "Manage User Secrets" UI, or  the .NET CLI:
//   cd <PROJECT-DIRECTORY>
//   dotnet user-secrets set OpenAI:Key <API-KEY>
//   dotnet user-secrets set OpenAI:Endpoint <ENDPOINT-URL>

var openAIClient = new OpenAIClient(
    credential: new ApiKeyCredential(builder.Configuration["OpenAI:Key"] ?? throw new InvalidOperationException("Missing configuration: OpenAI:Key. See the README for details.")),
    options: new OpenAIClientOptions { Endpoint = new Uri(builder.Configuration["OpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing configuration: OpenAI:Endpoint. See the README for details."))}
    );

#pragma warning disable OPENAI001 // GetResponsesClient() is experimental and subject to change or removal in future updates.
var chatClient = openAIClient.GetResponsesClient().AsIChatClient("gpt-5.4-mini");
#pragma warning restore OPENAI001

var embeddingGenerator = openAIClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

// Set the connection string to connect to Oracle AI Database in appsettings.json or in the .NET CLI
//   dotnet user-secrets set Oracle:ConnectionString "User Id=...;Password=...;Data Source=..."

var oracleConnectionString = builder.Configuration["Oracle:ConnectionString"];
if (string.IsNullOrWhiteSpace(oracleConnectionString))
{
    throw new InvalidOperationException("Missing configuration: Oracle:ConnectionString. See the README for the required Oracle database connection string.");
}

builder.Services.AddSingleton<OracleDataSource>(_ => new OracleDataSourceBuilder(oracleConnectionString).Build());
builder.Services.AddOracleVectorStore();
builder.Services.AddOracleCollection<Guid, IngestedChunk>(IngestedChunk.CollectionName);

builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddSingleton<ChatAgentTools>();
builder.Services.AddSingleton<ChatAgents>();
builder.Services.AddKeyedSingleton("ingestion_directory", new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "Data")));
builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
