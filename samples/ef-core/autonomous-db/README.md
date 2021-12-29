This sample code demonstrates using ODP.NET Entity Framework Core with an Oracle Autonomous Database (ADB). It builds off [this introductory sample](https://github.com/oracle/dotnet-db-samples/blob/master/samples/dotnet-core/ef-core/get-started/) for Oracle EF Core beginners. In this sample, ADB connection setup occurs in the OnConfiguring method. 

1. Set the OracleConfiguration.TnsAdmin property value to the tnsnames.ora and sqlnet.ora files directory location. 

2. Set the OracleConfiguration.WalletLocation property value to the ADB wallet directory location. Most Oracle Autonomous Databases require TCP with SSL (TCPS) connections for security purposes. ODP.NET uses the Oracle wallet, which stores the security credentials, to connect.

3. Set the connection string with the user id, password, and data source. The data source is commonly populated with the ADB TNS name value.
