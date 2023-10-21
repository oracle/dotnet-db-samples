Managed ODP.NET and ODP.NET Core Code Samples
=============================================
You must have managed ODP.NET or ODP.NET Core installed. To run the samples, follow these directions:
1) Modify the Data Source attribute in the connection strings to connect to an Oracle database via Easy Connect (Plus), TNS connect descriptor, or TNS alias.
2) Most of these samples use the SCOTT or Human Resources (HR) schema. <br>
The SCOTT schema create scripts are located here: https://github.com/oracle/dotnet-db-samples/tree/master/schemas <br>
The HR schema create scripts are located here: https://github.com/oracle/db-sample-schemas
3) Add Oracle.ManagedDataAccess.dll to the sample application.
4) Review the README file in the samples subdirectory, if any.

While these samples are designed for managed ODP.NET or ODP.NET Core, they generally can use unmanaged ODP.NET by incorporating Oracle.DataAccess.dll and 
adding the unmanaged ODP.NET namespace reference (i.e. "using Oracle.DataAccess.Client;" and "using Oracle.DataAccess.Types;").

Running ODP.NET Core Samples from Command Line
==============================================
1) Install .NET Core SDK from Microsoft's website: https://dotnet.microsoft.com/download
2) Open a terminal such as PowerShell, command prompt, or bash. Enter the following commands to create and setup your ODP.NET Core sample: <br>
  A) dotnet new console --output (Sample Name) <br>
  B) dotnet add package Oracle.ManagedDataAccess.Core --version (e.g. 3.21.120)
4) Replace the contents of Program.cs with the GitHub sample code of interest.
5) Insert your user id, password, and data source. The sample will have its own README or comments to indicate additional configuration that may be required.
6) Run using the following command: dotnet run --project (Sample Name)


Below is the feature list the samples cover. Each feature's sample has its own subdirectory.

Application Continuity
----------------------
Sample 1: Unmanaged ODP.NET Application Continuity code sample with setup and runtime demo instructions.

Parameter Array Binding
----------------------
Sample 1: Demonstrates parameter array binding.

ASP.NET Core
------------
ASP.NET Core 2.x: Demonstrates a simple ASP.NET Core 2.x web app to connect and retrieve data. <br>
ASP.NET Core 3.x: Demonstrates a simple ASP.NET Core 3.x web app to connect and retrieve data. <br>
ASP.NET Core 6: Demonstrates a simple ASP.NET Core 6 web app to connect and retrieve data. 

PL/SQL Associative Array
------------------------
Sample 1: Demonstrates PL/SQL Associative Array binding.

Autonomous Database
-------------------
ODP.NET Core Samples: Demonstrates how to connect ODP.NET Core to Oracle Autonomous Database via a console and an ASP.NET Core web app. <br>
Managed ODP.NET Samples: Demonstrates how to connect managed ODP.NET to Oracle Autonomous Database via a console and an ASP.NET web app. <br>
Unmanaged ODP.NET Sample: Demonstrates how to connect unmanaged ODP.NET to Oracle Autonomous Database via a console app.

Azure Active Directory
----------------------
Demonstrates connecting to Oracle Autonomous Database using an Azure Active Directory token with ODP.NET Core, managed, and unmanaged.

Bulk Copy
---------
Sample 1: Demonstrates how to use ODP.NET bulk copy. Sample works for both managed and core ODP.NET.

Client Factory
--------------
Sample 1: Demonstrates how to use the OracleClientFactory class.

Command Builder
---------------
Sample 1: Demonstrates OracleCommandBuilder's SchemaSeparator property. <br>
Sample 2: Demonstrates OracleCommandBuilders's QuoteIdentifier method. <br>
Sample 3: Demonstrates OracleCommandBuilders's UnquoteIdentifier method.

Configuration API
-----------------
Samples demonstrate how to use the OracleConfiguration, OracleDataSourceCollection, and OracleOnsServerCollection classes.

Connection
----------
Sample 1: Demonstrates OracleConnection's GetSchema() method. <br>
Sample 2: Demonstrates all variations of OracleConnection's GetSchema(string) method overload. <br>
Sample 3: Demonstrates all variations of OracleConnection's GetSchema(string, string[]) method overload.

Connection String Builder
-------------------------
Sample 1: Demonstrates how to use the OracleConnectionStringBuilder class.

DataReader
----------
Unmanaged ODP.NET Sample: Demonstrates OracleDataReader's VisibleFieldCount and HiddenFieldCount properties. <br>
ODP.NET Core Sample: Demonstrates how to connect and retrieve data using ODP.NET Core via a console app.

Data Source Enumerator
----------------------
Sample 1: Demonstrates the functionality of OracleDataSourceEnumerator class.

DataSet
-------
Sample 1: Demonstrates data manipulation language (DML) operations on a Dataset. <br>
Sample 2: Demonstrates how to populate a DataSet using C#. <br>
Sample 3: Demonstrates DML operations on LOB columns. <br>
Sample 4: Demonstrates how to populate a DataSet from multiple output Ref Cursors from a stored procedure. <br>
Sample 5: Demonstrates how to populate a DataSet using Visual Basic .NET (VB.NET).

Entity Framework Core
---------------------
Autonomous Database Sample: Demonstrates Oracle EF Core connecting to Oracle Autonomous Database. <br>
Dependency Injection Sample: Demonstrates using dependency injection and ASP.NET Core with Oracle EF Core. <br>
Getting Started Sample: Demonstrates a basic Oracle EF Core scenario using migrations and scaffolding. <br>
Keyless Entity Types Sample: Demonstrates Oracle EF Core keyless entity types with relational and materialized views. <br>
Stored Procedure Result Set Samples: Demonstrates using PL/SQL that returns either an explicitly or implicitly bound REF Cursor. 

Event Handler
-------------
Sample 1: Demonstrates how to trap the OracleRowUpdatingEvent and OracleRowUpdatedEvent using VB.NET.

Oracle Identity and Access Management
-------------------------------------
Sample 1: Demonstrates how to use OCI .NET SDK to retrieve, authenticate, and refresh Oracle database tokens.

JSON
----
Select JSON Sample: Demonstrates row insert into and query against a JSON table. <br>
Select JSON CLOB Sample:  Demonstrates row insert into and query against a JSON table using CLOB storage.

LOB 
---
Sample 1: Demonstrates how to populate and obtain LOB data from a DataSet. <br>
Sample 2: Demonstrates how an OracleClob object is obtained as an output parameter of an anonymous PL/SQL block. <br>
Sample 3: Demonstrates how an OracleClob object is obtained from an output parameter of a stored procedure. <br>
Sample 4: Demonstrates how the LOB column data can be read as a .NET type by utilizing stream reads. <br>
Sample 5: Demonstrates how to bind an OracleClob object as a parameter and refetch the newly updated CLOB data using an OracleDataReader and an OracleClob object. <br>
Sample 6: Demonstrates LOB updates using row-level locking. <br>
Sample 7: Demonstrates LOB updates using result set locking. <br>
BFile Sample: Demonstrates accessing BFILEs through ODP.NET.

OpenTelemetry
-------------
Sample: Demonstrates using managed ODP.NET or ODP.NET Core with OpenTelemetry

Performance Counters
--------------------
Sample 1: Demonstrates how to programmatically use ODP.NET performance counters.

Ref Cursor
----------
Sample 1: Demonstrates how a REF Cursor is obtained as an OracleDataReader. <br>
Sample 2: Demonstrates how a REF Cursor is obtained as an OracleDataReader through the use of an OracleRefCursor object. <br>
Sample 3: Demonstrates how multiple REF Cursors can be accessed by a single OracleDataReader. <br>
Sample 4: Demonstrates how a DataSet can be populated from a REF Cursor. The sample also demonstrates how a REF Cursor can be updated. <br>
Sample 5: Demonstrates how a DataSet can be populated from an OracleRefCursor object. <br>
Sample 6: Demonstrates how to populate a DataSet with multiple REF Cursors selectively. <br>
Sample 7: Demonstrates how to selectively obtain OracleDataReader objects from REF Cursors.

Statement Cache
---------------
Sample 1: Demonstrates performance improvement when statement caching is enabled.

Transaction
-----------
Sample 1: Demonstrates the usage of EnlistTransaction API. <br>
Sample 2: Demonstrates the usage of TransactionScope. <br>
Sample 3: Demonstrates nested transactions with savepoints.

User-Defined Types (UDT)
------------------------
Nested Table Sample: Demonstrates how to map, fetch, and manipulate a nested table of UDTs that has an inheritance hierarchy (i.e. parent and child types). <br>
Object UDT Sample: Demonstrates how to map, fetch, and manipulate an Oracle UDT as a .NET custom object. <br>
Spatial UDT Sample: Demonstrates how to map and fetch types similar to Oracle Spatial types as custom types. <br>
Ref Sample: Demonstrates how to fetch UDTs referenced by REFs. <br>
Ref Inheritance Sample: Demonstrates how to obtain and update Custom Type objects from OracleRef objects. <br>
VARRAY Sample: Demonstrates how to map, fetch, and manipulate the Oracle VARRAY as a custom object. 
