# Oracle AI Chat .NET Web App using Oracle AI Database and Microsoft Agent Framework

This Oracle AI web application uses a large language model and Oracle AI Database to answer questions about a custom data set with retrieval-augmented generation (RAG).

The app uses Oracle Database Vector Store Connector, [`Oracle.VectorData`](https://www.nuget.org/packages/Oracle.VectorData/), to provide a seamless experience for incorporating .NET vectors and Microsoft Agent Framework. The chat experience uses the framework's `ChatClientAgent` and `AgentSession` APIs. The agent is backed by an OpenAI-compatible `IChatClient`, exposes document loading and semantic search as tools, and maintains conversation state through an agent session. Microsoft.Extensions.AI remains in use for the provider client, embeddings, vector search, and shared chat content types.

Two documents are chunked with their embeddings stored in Oracle AI Database through the connector. One document is a PDF about an emergency survival kit. The second is a Markdown file about a GPS watch.

Data ingestion uses [`Microsoft.Extensions.DataIngestion`](https://learn.microsoft.com/en-us/dotnet/ai/conceptual/medi-library) library to apply semantic chunking and a string-length processor to conform to `Oracle.VectorData` connector's default .NET `string` mapping of `NVARCHAR2(2000)`.

## Use the App
Documents are ingested on the first agent tool-use request, rather than during application startup. The application then performs the following steps:
1. Retrieves the files under the documents directory `wwwroot/Data`.
2. Splits each document using semantic similarity chunking and string-length processor.
3. Generates embeddings through the configured embedding model.
4. Upserts the chunks and embeddings into the database.

Concurrent requests share one ingestion attempt, and ingestion runs again after an application restart. 

Now, RAG queries can be run against the database using natural language questions. OpenAI will return the natural language answers using the vector connector and Oracle AI vector search.

When delivering responses, the app cites the document that provided its answer and suggests several follow up questions.

## Configure the Oracle AI .NET Chat App
### Connection String and API Key and Endpoint with `appsettings.json`

In `appsettings.json`, set the connection string values, including `User Id`, `Password`, and `Data Source`. You can use an on-premises or cloud database, such as Oracle Autonomous AI Database. The database must have AI vector support. Oracle AI Database 26ai is recommended.

Next, set your OpenAI API key and endpoint in the same file.

As a reminder, keep real credentials out of source control.

Alternatively, all these values can be set using Visual Studio's "Manage User Secrets" UI or on the .NET Command-Line Interface (CLI).

### Configure Oracle AI Database Vector Store with .NET CLI

Configure the database connection string used by `Oracle.VectorData` connector using .NET user secrets:

```sh
cd <PROJECT-DIRECTORY>
dotnet user-secrets init
dotnet user-secrets set Oracle:ConnectionString "<CONNECTION-STRING>"
```

### Using OpenAI or an OpenAI-Compatible API Key and Endpoint with .NET CLI

To call the OpenAI REST API, you will need an API key. To obtain one, first [create a new OpenAI account](https://platform.openai.com/signup) or [log in](https://platform.openai.com/login). Next, navigate to the API key page and select "Create new secret key", optionally naming the key. Make sure to save your API key somewhere safe and do not share it with anyone.

From the command line, configure your API key for this project using .NET User Secrets:

```sh
cd <PROJECT-DIRECTORY>
dotnet user-secrets init
dotnet user-secrets set OpenAI:Key <API-KEY>
dotnet user-secrets set OpenAI:Endpoint <ENDPOINT-URL>
```

### Using OCI Generative AI API Key and Endpoint with .NET CLI

OCI Generative AI (GenAI) [API Keys](https://docs.oracle.com/en-us/iaas/Content/generative-ai/api-keys.htm) are secure credential tokens used to authenticate callers and authorize access to large language models hosted by Oracle Cloud Infrastrcture compatible with OpenAI clients. You can access [select Chat models](https://docs.oracle.com/en-us/iaas/Content/generative-ai/api-keys.htm#supported-models-openai) from the OCI GenAI service using API keys. Currently, embedding models are not accessible using API keys.

You can configure your OCI key and endpoint from the .NET CLI.
```sh
cd <PROJECT-DIRECTORY>
dotnet user-secrets init
dotnet user-secrets set OpenAI:Key <API-KEY>
dotnet user-secrets set OpenAI:Endpoint https://inference.generativeai.<REGION-IDENTIFIER>.oci.oraclecloud.com/20231130/actions/v1
```

## Application AI Default Settings
The encoding tokenizer is set to create a model based on OpenAI's `gpt-5` in `DataIngestor.cs`.

During ingestion, the default vector dimensions are set to 1536 and vector distance function uses cosine distance in `IngestedChunk.cs`.

The OpenAI response client uses the `gpt-5.4-mini` reasoning model and embedding client `text-embedding-3-small` by default. These are set in `Program.cs`. 
