# Oracle AI Vector Hotel Search Sample

This .NET console application demonstrates vector storage and similarity search for hotel data with Oracle AI Database and the [`Oracle.VectorData`](https://www.nuget.org/packages/Oracle.VectorData/) connector.

The demo application reads sample hotel records from `Hotels.json`, generates 384-dimensional embeddings in the database with the `ALL_MINILM_L12_V2` ONNX model, and stores the records in a vector collection. 

It then demonstrates four searches:

1. Retrieve hotels by primary key.
2. Filter hotels by a scalar property, such as a rating of 9 or higher.
3. Search hotel names by cosine distance.
4. Search hotel descriptions by Euclidean distance.

During cleanup or upon an exception, the application attempts to delete the collection and the corresponding database table.

## Configure the database connection

Edit `AppSettings.json` and set `Oracle:ConnectionString` to a connection string for your database:

```json
{
  "Oracle": {
    "ConnectionString": "User Id=<USER>; Password=<PASSWORD>; Data Source=<DATA-SOURCE>;"
  }
}
```

As a reminder, keep real credentials out of source control.
