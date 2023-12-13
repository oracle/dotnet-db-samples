# Getting Started with Oracle Entity Framework Core

There are two sets of sample code showing how to perform several basic tasks with Oracle EF Core. 
1. [Getting Started with Oracle EF Core 6 and higher](https://github.com/oracle/dotnet-db-samples/blob/master/samples/ef-core/get-started/create-model-save-query-scaffold-efc.cs)
2. [Getting Started with Oracle EF Core 5 and earlier releases](https://github.com/oracle/dotnet-db-samples/blob/master/samples/ef-core/get-started/create-model-save-query-scaffold.cs)

The sample code and below instructions show how to create a data model, save it to an Oracle database, insert data, query the data, use migrations to modify the schema, and reverse engineer.

Oracle Entity Framework Core is currently available on [nuget.org](https://www.nuget.org/packages/Oracle.EntityFrameworkCore/).

To use the sample code, create a new Oracle user. Log into the Oracle Database with DBA privileges and run the following commands:

* GRANT CONNECT,RESOURCE,UNLIMITED TABLESPACE TO blog IDENTIFIED BY blog;
* ALTER USER blog DEFAULT TABLESPACE USERS;
* ALTER USER blog TEMPORARY TABLESPACE TEMP;

In Visual Studio, create a .NET Core console project. In NuGet, install the following packages to the project:
* Oracle.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Tools

Copy and paste the Oracle Entity Framework Core C# source code sample.
Modify the Data Source entry so that the application can connect to the database with the new user.

Save the project, including the C# source file (.cs) and project file (.csproj).

Open the Package Manager Console (PMC) in Visual Studio and enter the following commands:
* add-migration first
* update-database

These commands will scaffold the migration, create an initial model for the two tables, then apply the migrations to the database.
When you navigate to the database and view the Blog schema, you will now see the Blog table and Posts table.

You can then run the application, which will add a new blog row to the Blogs table.

Finally, you can uncomment the Rating property in the Blog class, then run in the PMC:
* add-migration second
* update-database

You will now see a new Rating column in the Blog table.

To generate .NET code for a DbContext and entity types from a database, run the following command in the PMC:

* Scaffold-DbContext "User Id=blog;Password=\<Password>;Data Source=\<Data Source>;" Oracle.EntityFrameworkCore -OutputDir Models

A "Models" directory will be created in your Visual Studio project with EF Core generated code based on the Oracle schema.
