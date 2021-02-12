# ODP.NET Client Initiated Continuous Query Notification and JSON Data Type Code Sample
This C# sample console app uses ODP.NET Core 21c to demonstrate two new Oracle Database 21c features:
* Client Initiated Continuous Query Notification (CICQN) 
* Native JSON binary data type (OSON)

Learn more about ODP.NET CICQN and JSON data type in this blog post.

These features require Oracle Database 21c and ODP.NET 21c or higher. You can download 
[ODP.NET 21c from NuGet Gallery](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/3.21.1). 
Oracle Database 21c can be deployed in minutes using a [free Oracle Autonomous Database](https://www.oracle.com/cloud/free/).

Setup instructions: 
1. The database administrator executes "GRANT CHANGE NOTIFICATION TO <USER>" so that the app can use CICQN.
2. Run the setup.sql file scripts to create the J_PURCHASEORDER table and insert a JSON document into the user's schema.
3. Populate the Constants class in the C# app with your app-specific values.

The sample code retrieves the JSON data and shows the results on the console. While the app is idle, 
another user can update the JSON data by running the following update statement in SQL Developer or Oracle 
Developer Tools for Visual Studio.

```
UPDATE j_purchaseorder SET po_document = json_transform(po_document, SET '$.LastUpdated' = SYSDATE);
```

CICQN will then notify the application an update to the result occurred. The event handler will then re-query the database
for the new results. It will then display the results with the new LastUpdated JSON value.

The update statement can be run repeatedly with the new JSON update returned every time.
