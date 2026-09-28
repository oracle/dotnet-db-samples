# Oracle AI Chat .NET Web App using Oracle AI Database and Microsoft Agent Framework

This Oracle AI web application uses a large language model and Oracle AI Database to answer questions about a custom data set with retrieval-augmented generation (RAG).

The app uses Oracle Database Vector Store Connector, <a href="https://www.nuget.org/packages/Oracle.VectorData/">`Oracle.VectorData`</a>, to provide a seamless experience for incorporating .NET vectors and Microsoft Agent Framework. The chat experience uses the framework's `ChatClientAgent` and `AgentSession` APIs. The agent is backed by an OpenAI-compatible `IChatClient`, exposes document loading and semantic search as tools, and maintains conversation state through an agent session. Microsoft.Extensions.AI remains in use for the provider client, embeddings, vector search, and shared chat content types.

Two documents are chunked with their embeddings stored in Oracle AI Database through the connector. One document is a PDF about an emergency survival kit. The second is a Markdown file about a GPS watch.

Data ingestion applies a character-length processor after semantic chunking to conform to `Oracle.VectorData` connector's default .NET `string` mapping of `NVARCHAR2(2000)`.

## Use the App
When you run the app, it first ingests the custom data set into the Oracle database and prepares it for AI vector search. The app performs the following setup steps:
1.	Retrieves the files in the documents directory.
2.	Obtains the file list already ingested using the Oracle vector connector.
3.	The connector updates the Oracle database table with the current file list.
4.	The app chunks the text of the new files.
5.	The connector inserts those chunks into the database.
6.	Embeddings are generated using the OpenAI model.
7.	The connector updates the database with text chunks and embeddings.

Now, RAG queries can be run against the database using natural language questions. OpenAI will return the natural language answers using the vector connector and Oracle AI vector search.

When delivering responses, the app cites the document that provided its answer and suggests several follow up questions.

## Configure the Oracle AI .NET Chat App
### Connection String and API Key and Endpoint with `appsettings.json`

In `appsettings.json`, set the connectiong string values, including `User Id`, `Password`, and `Data Source`. You can use an on-premises or cloud database, such as Oracle Autonomous AI Database. The database must have AI vector support. Oracle AI Database 26ai is recommended.

Next, set your OpenAI API key and endpoint in the same file.

Alternatively, all these values can be set using Visual Studio's "Manage User Secrets" UI or on the .NET Command-Line Interface (CLI).

### Using OCI Generative AI API Key and Endpoint with .NET CLI

OCI Generative AI (GenAI) [API Keys](https://docs.oracle.com/en-us/iaas/Content/generative-ai/api-keys.htm) are secure credential tokens used to authenticate callers and authorize access to large language models hosted by Oracle Cloud Infrastrcture. You can access [select Chat models](https://docs.oracle.com/en-us/iaas/Content/generative-ai/api-keys.htm#supported-models-openai) from the OCI GenAI service using API keys. Currently, embed models are not accessible using API keys.

You can configure your OCI key and endpoint from the .NET CLI.
```sh
cd <PROJECT-DIRECTORY>
dotnet user-secrets set OpenAI:Key <API-KEY>
dotnet user-secrets set OpenAI:Endpoint https://inference.generativeai.<REGION-IDENTIFIER>.oci.oraclecloud.com/20231130/actions/v1

```

### Using OpenAI or an OpenAI-Compatible API Key and Endpoint with .NET CLI

To call the OpenAI REST API, you will need an API key. To obtain one, first [create a new OpenAI account](https://platform.openai.com/signup) or [log in](https://platform.openai.com/login). Next, navigate to the API key page and select "Create new secret key", optionally naming the key. Make sure to save your API key somewhere safe and do not share it with anyone.

From the command line, configure your API key for this project using .NET User Secrets:

```sh
cd <PROJECT-DIRECTORY>
dotnet user-secrets set OpenAI:Key <API-KEY>
dotnet user-secrets set OpenAI:Endpoint <ENDPOINT-URL>
```

## Configure Oracle AI Database Vector Store with .NET CLI

Configure the database connection string used by `Oracle.VectorData` connector using .NET user secrets:

```sh
cd <PROJECT-DIRECTORY>
dotnet user-secrets set Oracle:ConnectionString "<CONNECTION-STRING>"
```

## Application AI Default Settings
The encoding tokenizer is set to create a model based on OpenAI's `gpt-5` in `DataIngestor.cs`.

During ingestion, the default vector dimensions are set to 1536 and vector distance function uses cosine distance in `IngestedChunk.cs`.

The OpenAI chat client uses the GPT-5.4 Mini reasoning model and embedding client text-embedding-3-small by default. These are set in `Program.cs`. 
