This C# sample code shows how to use Oracle Entity Framework Core with keyless entity types and views. This sample works with Oracle EF Core 3.1.

Oracle Entity Framework Core is available for free download on [NuGet Gallery](https://www.nuget.org/packages/Oracle.EntityFrameworkCore/).

To use the sample code, enter the User Id, Password, and Data Source values for the Oracle connection string. 

Ensure the EF Core user has the database privileges to create the necessary schema objects to run the sample code. If you have granted the privileges in the [Oracle EF Core Getting Started sample](https://github.com/oracle/dotnet-db-samples/tree/master/samples/dotnet-core/ef-core/get-started) already, then you only have to grant following privilege to allow view creation:
* GRANT CREATE VIEW TO "&lt;Oracle User&gt;"
