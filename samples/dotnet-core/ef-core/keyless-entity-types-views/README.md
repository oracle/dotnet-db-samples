This C# sample code shows how to use Oracle Entity Framework Core with keyless entity types with relational and materialized views. It auto-generates the necessary schema objects.

This sample uses Oracle EF Core 3.1 provider, which is available as a free download on [NuGet Gallery](https://www.nuget.org/packages/Oracle.EntityFrameworkCore/).

To use the sample code, enter the User Id, Password, and Data Source values for the Oracle database connection string. 

Ensure the EF Core user has the database privileges to create the necessary schema objects to run the sample code. If you have granted the privileges in the [Oracle EF Core Getting Started sample](https://github.com/oracle/dotnet-db-samples/tree/master/samples/dotnet-core/ef-core/get-started) already, then you only need to grant view creation privileges:
* GRANT CREATE VIEW TO "&lt;Oracle User&gt;"
* GRANT CREATE MATERIALIZED VIEW TO "&lt;Oracle User&gt;"

To scaffold views, add the Microsoft.EntityFrameworkCore.Tools package from NuGet Gallery. Then, run this command from the Package Manager Console:
* Scaffold-DbContext "User Id=&lt;Oracle User&gt;;Password=&lt;Password&gt;;Data Source=&lt;Data Source&gt;;" Oracle.EntityFrameworkCore -OutputDir Models
