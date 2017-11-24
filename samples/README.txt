ODP.NET, Managed Driver Code Samples
====================================

Below is a list of topics that the samples cover along with their locations:

To run the samples, follow these directions:
1) Modify the Data Source attribute in the connection strings to connect to an Oracle database via Easy Connect, TNS connect descriptor, or TNS alias.
2) Most of these samples use the SCOTT schema. The create scripts for SCOTT schema are located here: https://github.com/oracle/dotnet-db-samples/tree/master/schemas
3) Add Oracle.ManagedDataAccess.dll to the sample application.
4) Read <GitHub .NET samples directory>\doc\Readme.html, if any. 


Parameter Array Binding
=======================
Sample 1: Demonstrates parameter array binding.

PL/SQL Associative Array
========================
Sample 1: Demonstrates PL/SQL Associative Array binding.

Client Factory
==============
Sample 1: Demonstrates how to use the OracleClientFactory class.

Command Builder
===============
Sample 1: Demonstrates CommandBuilder's SchemaSeparator property.
Sample 2: Demonstrates CommandBuilders's QuoteIdentifier method.
Sample 3: Demonstrates CommandBuilders's UnquoteIdentifier method.

Connection
==========
Sample 1: Demonstrates OracleConnection's GetSchema() method.
Sample 2: Demonstrates all variations of OracleConnection's GetSchema(string) method overload.
Sample 3: Demonstrates all variations of OracleConnection's GetSchema(string, string[]) method overload.

Connection String Builder
=========================
Sample 1: Demonstrates how to use the ConnectionStringBuilder class.

Data Reader
===========
Sample 1: Demonstrates OracleDataReader's VisibleFieldCount and HiddenFieldCount properties.

Data Source Enumerator
======================
Sample 1: Demonstrates the functionality of OracleDataSourceEnumerator class.

DataSet
=======
Sample 1: Demonstrates data manipulation language (DML) operations on a Dataset.
Sample 2: Demonstrates how to populate a DataSet using C#.
Sample 3: Demonstrates DML operations on LOB columns.
Sample 4: Demonstrates how to populate a DataSet from multiple output Ref Cursors from a stored procedure.
Sample 5: Demonstrates how to populate a DataSet using Visual Basic .NET (VB.NET).

Event Handler
=============
Sample 1: Demonstrates how to trap the OracleRowUpdatingEvent and OracleRowUpdatedEvent using VB.NET.

LOB 
===
Sample 1: Demonstrates how to populate and obtain LOB data from a DataSet.
Sample 2: Demonstrates how an OracleClob object is obtained as an output parameter of an anonymous PL/SQL block.
Sample 3: Demonstrates how an OracleClob object is obtained from an output parameter of a stored procedure.
Sample 4: Demonstrates how the LOB column data can be read as a .NET type by utilizing stream reads.
Sample 5: Demonstrates how to bind an OracleClob object as a parameter.  This sample also refetches the newly updated CLOB data using an OracleDataReader and an OracleClob object.
Sample 6: Demonstrates LOB updates using row-level locking.
Sample 7: Demonstrates LOB updates using result set locking.
BFile Sample: Demonstrates accessing BFILEs through ODP.NET.

Ref Cursor
==========
Sample 1: Demonstrates how a REF Cursor is obtained as an OracleDataReader.
Sample 2: Demonstrates how a REF Cursor is obtained as an OracleDataReader through the use of an OracleRefCursor object.
Sample 3: Demonstrates how multiple Ref Cursors can be accessed by a single OracleDataReader.
Sample 4: Demonstrates how a DataSet can be populated from a Ref Cursor. The sample also demonstrates how a Ref Cursor can be updated.
Sample 5: Demonstrates how a DataSet can be populated from an OracleRefCursor object.
Sample 6: Demonstrates how to populate a DataSet with multiple Ref Cursors selectively.
Sample 7: Demonstrates how to selectively obtain OracleDataReader objects from Ref Cursors.

Statement Cache
===============
Sample 1: Demonstrates performance improvement when statement caching is enabled.

Transaction
===========
Sample 1: Demonstratew the usage of EnlistTransaction API.
Sample 2: Demonstrates the usage of TransactionScope.
Sample 3: Demonstrates nested transactions with savepoints.
