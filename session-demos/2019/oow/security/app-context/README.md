This sample application shows how to use ODP.NET application context and its properties. This feature is also known as end to end user tracing. It is a C# console app using managed ODP.NET.

In this app, four application context properties are set after the connection opens:
* ClientId
* ActionName
* ModuleName
* ClientInfo

These context values can be used by database application logic for purposes, such as access control, auditing, resource usage tracking, etc. The following query shows the context values in the database:
* SELECT ACTION, CLIENT_IDENTIFIER, CLIENT_INFO, MODULE, USERNAME FROM V$SESSION WHERE USERNAME='HR';
