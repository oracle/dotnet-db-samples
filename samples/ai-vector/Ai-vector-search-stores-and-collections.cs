using Oracle.VectorData;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.VectorData;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace OracleAIVectorData;

// This ODP.NET sample app shows various ways to search vector data and collections.
// First, the data (hotels.json) is loaded into a .NET object list.
// The data initially includes hotel names, descriptions, ids, ratings, and parking availability.
// The hotel names and descriptions are then vectorized using the database ONNX embeddings, consisting of 384 dimensions in Float format.
// The entire data set is upserted into the database.
// Finally, four search operations are demonstrated.
//  1. Search by primary key.
//  2. Search scalar value properties.
//  3. Similarity search using cosine similarity.
//  4. Similarity search using Euclidean distance.
// This sample requires Oracle Database 23ai or higher.
// Add Oracle.VectorData and Microsoft.Extensions.Configuration.Json NuGet packages to your project.

public class AIHotelSearchApp
{
    public static async Task Main(string[] args)
    {
        await SearchHotels();
    }
    static async Task SearchHotels()
    {
        // Setup ODP.NET connection and vector configuration.
        // Set connection string values in AppSettings.json file.
        // Add AppSettings.json directory path below or place file in app's output directory.
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(path: "AppSettings.json", optional: false)
            .Build();

        string? connStr = configuration.GetSection("Oracle")["ConnectionString"];
        OracleDataSource? ds = null;
        OracleVectorStore? vs = null;
        OracleCollection<int, Hotel>? collection = null;
        string collectionName = "Hotels";

        try
        {
            ds = new OracleDataSourceBuilder(connStr).Build();

            // Create a vector store
            vs = new OracleVectorStore(ds);

            // Create a vector collection
            collection = (OracleCollection<int, Hotel>)vs.GetCollection<int, Hotel>(collectionName);

            // HotelsData.json contains plain text information about various hotels.
            // Add Hotels.json directory path below or place file in app's output directory.
            string jsonContent = File.ReadAllText("Hotels.json");
            List<Hotel>? hotels = JsonSerializer.Deserialize<List<Hotel>>(jsonContent);

            // Use the database ONNX generator to create VECTOR(384, FLOAT32) embeddings for each hotel/record.
            foreach (Hotel hotel in hotels)
            {
                hotel.NameEmbedding = await GenerateEmbeddingAsync(ds, hotel.HotelName);
                hotel.DescriptionEmbedding = await GenerateEmbeddingAsync(ds, hotel.Description);
            }

            // Verify the collection exists in the database.
            await collection.EnsureCollectionExistsAsync();

            // Upsert the records into the database.
            await collection.UpsertAsync(hotels);

            // Search hotels in the vector collection by primary key.
            Console.WriteLine("Search for hotels with ID 5 and 10.");
            Console.WriteLine("===============================================================================");
            IAsyncEnumerable<Hotel> hotelsById = collection.GetAsync([5, 10]);
            await foreach (Hotel hotel in hotelsById)
            {
                Output(hotel);
            }
            Console.WriteLine();

            // Search hotels by their characteristics, such as rating and parking availability.
            Console.WriteLine("Search for hotels with a 9 or higher rating and parking.");
            Console.WriteLine("===============================================================================");
            IAsyncEnumerable<Hotel> hotelsByFilter2 = collection.GetAsync(r => r.Rating >= 9 && r.HasParking == true, 3);
            await foreach (Hotel hotel in hotelsByFilter2)
            {
                Output(hotel);
            }
            Console.WriteLine();

            // Search hotels by their names. Return top three most similar matches.
            // Provide a search term, such as "beach". Generate a vector embedding using the search term.
            // ODP.NET performs a similarity search using the hotel name and search term embeddings.
            // The cosine similarity metric is used to calculate vector distances to find the best matches. 
            // Scores closer to zero are more similar. Higher scores mean more dissimilarity.
            // Results are ranked from most similar to least.
            string hotelNameSearchStr = "beach";

            var nameEmbedding = await GenerateEmbeddingAsync(ds, hotelNameSearchStr);
            // Specify the search option for hotel name.
            VectorSearchOptions<Hotel> nameOptions = new() { VectorProperty = r => r.NameEmbedding };
            IAsyncEnumerable<VectorSearchResult<Hotel>> namesVectorSearch = collection.SearchAsync(nameEmbedding, top: 3, nameOptions);

            int rank = 1;
            Console.WriteLine($"Hotel name similarity search with \"{hotelNameSearchStr}\".");
            Console.WriteLine("===============================================================================");
            await foreach (VectorSearchResult<Hotel> searchResult in namesVectorSearch)
            {
                Console.WriteLine(rank + $". {searchResult.Record.HotelName}");
                Console.WriteLine($"Score : {searchResult.Score}");
                Console.WriteLine();
                rank++;
            }
            Console.WriteLine();

            // Search hotels using their descriptions. Return top three most similar matches.
            // Provide a search phrase or sentence. Generate its vector embedding.
            // ODP.NET performs a similarity search using the hotel description and search text embeddings.
            // The Euclidean distance metric is used to calculate vector distances to find the best matches. 
            string descriptionSearchStr = "I want a hotel with nature activities.";
            var descriptionEmbedding = await GenerateEmbeddingAsync(ds, descriptionSearchStr);

            // Specify the search option for hotel description.
            VectorSearchOptions<Hotel> descriptionOptions = new() { VectorProperty = r => r.DescriptionEmbedding };
            IAsyncEnumerable<VectorSearchResult<Hotel>> descriptionVectorSearch = collection.SearchAsync(descriptionEmbedding, top: 3, descriptionOptions);

            rank = 1;
            Console.WriteLine($"Hotel description similarity search with \"{descriptionSearchStr}\".");
            Console.WriteLine("===============================================================================");
            await foreach (VectorSearchResult<Hotel> searchResult in descriptionVectorSearch)
            {
                Console.WriteLine(rank + $". {searchResult.Record.HotelName}");
                Console.WriteLine($"Score : {searchResult.Score}");
                Console.WriteLine($"Description: {searchResult.Record.Description}");
                Console.WriteLine();
                rank++;
            }
        }

        finally
        {
            // Clean up and delete the collection
            if (vs != null) { await vs.EnsureCollectionDeletedAsync(collectionName); }
            ds?.Dispose();
            vs?.Dispose();
            collection?.Dispose();
        }
    }

    // Generate embeddings in ONNX format.
    // This app uses Hugging Face's all-MiniLM-L12-v2 model for all its embeddings.
    static async Task<float[]> GenerateEmbeddingAsync(OracleDataSource ds, string searchText, CancellationToken cancellationtoken = default)
    {
        using (OracleConnection conn = await ds.OpenConnectionAsync(cancellationtoken))
        {
            using (OracleCommand cmd = new OracleCommand($"SELECT TO_VECTOR(VECTOR_EMBEDDING(ALL_MINILM_L12_V2 USING :1 as DATA), 384, FLOAT32)", conn))
            {
                cmd.Parameters.Add("searchStr", OracleDbType.Varchar2, null, System.Data.ParameterDirection.Input);
                cmd.Parameters[0].Value = searchText;
                return (float[])cmd.ExecuteScalar();
            }
        }
    }

    // Output the hotel's information to the console.
    static void Output(Hotel hotel)
    {
        Console.WriteLine($"Hotel Name = {hotel.HotelName}");
        Console.WriteLine($"Hotel Id = {hotel.HotelId}");
        Console.WriteLine($"Rating = {hotel.Rating}");
        Console.WriteLine($"HasParking = {hotel.HasParking}");
        Console.WriteLine();
    }
}
/* Copyright (c) 2025 Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/