This sample application shows how ODP.NET connects to the database using proxy authentication. It is a C# console app using managed ODP.NET.

Log in as an administrator and execute the following SQL to setup the proxy users:
* create user appserver identified by appserver;
* grant connect, resource to appserver;
* alter user hr grant connect through appserver;

This sample assumes you have the HR sample schema already installed.
