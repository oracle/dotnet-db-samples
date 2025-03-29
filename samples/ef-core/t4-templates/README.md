# Customize Oracle EF Core Data Type Mappings with T4 Text Templates

Oracle EF Core data type mapping between entity properties and database columns can be customized with T4 text templates. This repository includes sample T4 templates that can be used as is or customized with an alternative set of .NET data type mappings. The samples demonstrate the following mapping scenarios:

* All Numeric Types - Customizes all database numeric column type mappings to .NET properties
* Single Numeric Type - Customizes one database column type mapping to a specific .NET property

The [ODP.NET Scaffolding documentation](https://docs.oracle.com/en/database/oracle/oracle-database/23/odpnt/EFCoreREDataTypeMapping.html) provides more information and a step-by-step usage guide.
