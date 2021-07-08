# Connecting Oracle Autonomous Database with Azure Web App
This ODP.NET Core C# sample demonstrates how to connect an ASP.NET Core Azure app to Oracle Autonomous Database (ADB).

Watch this [video to learn how to set up Azure and Oracle Autonomous Database](https://www.youtube.com/watch?v=DfGkBGuOv_c) with this sample code.

Both [ODP.NET Core](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/) and [Oracle Autonomous Database](https://docs.oracle.com/en-us/iaas/Content/Database/Concepts/adbfreeoverview.htm) are available for free to developers.

The sample queries the Sales History (SH) schema, which is available to all shared ADB instances from any connected user. If you are using dedicated ADB, replace the query 
with your own customized one. The app was tested using ODP.NET Core 21c and ASP.NET Core 5, but is expected to work with both older and newer ODP.NET and .NET versions.

Sample use directions:
1. Create a blank ASP.NET Core app. 
2. Add ODP.NET Core from NuGet Gallery.
3. Replace the contents of Startup.cs with the sample code.
4. Provide the app's namespace, user password, net service name, TNS admin directory, and wallet directory. Modify the user id if you are not using the standard ADMIN account. 
5. Run the web app from Visual Studio on-premises to connect to ADB. This step provides a checkpoint that the app and ADB are configured correctly.
6. Modify the TNS admin (for tnsnames.ora and sqlnet.ora) and wallet (for cwallet.sso) directory values to the Azure directories these files will be uploaded to.
7. Deploy the app to the Azure web app service. Select the Basic pricing level or higher. The Shared and Free levels cannot use file-based wallets, which this demo uses.
8. The tnsnames.ora, sqlnet.ora, and wallet files will not be deployed by default. Modify the .NET project file (i.e. csproj) so that they will be deployed. For example, if a directory "DB" is where these Oracle files are stored, make the following change:
```
  <ItemGroup>
    <None Include="DB\**" CopyToPublishDirectory="Always">
  </ItemGroup>
```
9. Once deployed, go to the Azure portal to manage the web app you just created. Click on <b>Configuration</b>, then click on <b>New application setting</b>. Add the <b>WEBSITE_LOAD_USER_PROFILE</b> setting with a value of 1. Click <b>OK</b> and then click <b>Save</b> to apply the new setting. This step allows the web server to use the file-based wallet.
10. Once Azure restarts the web server, click the <b>Browse</b> link.
  

Your web app should be able to connect ADB and see the Sales History data.
