This Entity Framework Core sample code demonstrates how to use Oracle PL/SQL stored procedures and return result sets from those stored procedures. Entity Framework Core stored procedures can be executed using anonymous PL/SQL and the FromSql extension method. 

The sample shows two different result set types: an explicitly bound REF Cursor and an implicitly bound REF Cursor. As the name implies, the former requires the REF Cursor parameter to be explicitly bound to the stored procedure statement. The implicit REF Cursor does not. There are two SQL files that set up the stored procedures:
* **return-implicit-ref-cursor-stored-procedure.sql** - creates a stored procedure returning an implicitly bound REF Cursor
* **return-ref-cursor-stored-procedure.sql** - creates a stored procedure returning an explicitly bound REF Cursor

To use this sample code, follow these steps:
1. Log in to the EF Core database schema to execute these two SQL files creating the stored prcocedures. 
2. Modify the user credentials and database connection information in the C# .NET Core console app, **return-ref-cursor.cs**.
3. Create an EF Core migration in the database from the sample code data model.
4. Run the application.
