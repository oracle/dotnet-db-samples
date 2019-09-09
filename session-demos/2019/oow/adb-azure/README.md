This managed ODP.NET web application shows how to connect to an Oracle Autonomous Database from either on-premises or Azure App Service.

Run this application from an ASP.NET Web project. The application logs into the HR schema. Prior to running this app:

1) Add managed ODP.NET (Oracle.ManagedDataAccess) from the NuGet Gallery.
2) Enter the HR password.
3) Enter the database net service name, Easy Connect (Plus), or equivalent connection information.
4) Enter the wallet directory and TNS_ADMIN directory of the deployment machine in the Web.config file.
