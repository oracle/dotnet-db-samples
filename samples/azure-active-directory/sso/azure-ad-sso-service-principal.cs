//This application demonstrates connecting .NET to Oracle database using Microsoft Entra ID/Azure Active
// Directory single sign-on (SSO). It uses service principal authentication with either managed ODP.NET or
// ODP.NET Core 23c or higher.

// ODP.NET Azure AD SSO requires Oracle.ManagedDataAccess.Azure package from NuGet Gallery
using Oracle.ManagedDataAccess.Azure;
using Oracle.ManagedDataAccess.Client;
using System.Security;

//Set your Azure Active Directory parameters below and ODP.NET data source value
string clientId = "<AZURE AD APP REGISTRATION CLIENT ID>";
string tenantId = "<AZURE AD TENANT ID>";
string clientSecret = "<AZURE AD APP REGISTRATION SECRET VALUE>";
string dbAppIdUri = "<AZURE AD PROTECTED RESOURCE ID>";
var conn = new OracleConnection("User Id=/;Data Source=<DATA SOURCE>;Connection Timeout=900;");

var secureSecret = new SecureString();
foreach (char c in clientSecret)
{
    secureSecret.AppendChar(c);
}
secureSecret.MakeReadOnly();

//Create Azure authentication token object and set its values.
var tokenConfig = new AzureTokenAuthentication
{
    ClientId = clientId,
    TenantId = tenantId,
    ClientSecret = secureSecret,
    DatabaseApplicationIdUri = dbAppIdUri,
};

//Set token authentication mode to Azure Service Principal and use Azure token authentication
conn.TokenAuthentication = OracleTokenAuth.AzureServicePrincipal;
conn.UseAzureTokenAuthentication(tokenConfig);

try
{
    conn.Open();
    Console.WriteLine("Connection opened successfully!");
    using (OracleCommand cmd = conn.CreateCommand())
    {
        //Retrieve authenticated identity value from database
        cmd.CommandText = "SELECT SYS_CONTEXT('USERENV', 'AUTHENTICATED_IDENTITY') FROM DUAL";
        Console.WriteLine($"Authenticated identity: {cmd.ExecuteScalar().ToString()}");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}
conn.Dispose();

/* Copyright (c) 2023, Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/dotnet-db-samples/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/
