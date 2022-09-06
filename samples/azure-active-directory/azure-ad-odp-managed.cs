// This is a simple ODP.NET, Managed Driver application that connects to an Oracle Autonomous Database
// using a token obtained from Azure Active Directory (Azure AD).

// Azure.Identity can be obtained through NuGet Gallery.
// It will include the Azure.Core and Azure.Identity namespaces.
using System;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Oracle.ManagedDataAccess.Client;

namespace ConnectToOracleUsingAccessToken
{
  class Program
  {
    static void Main()
    {
      try
      {
        // Retrieve an access token from Azure AD.
        string token = GetAccessToken();

        // Create an instance of an OracleAccessToken.  The access token needs to
        // be passed to the OracleAccessToken constructor as array of characters.
        var oracleAccessToken = new OracleAccessToken(token.ToCharArray());

        // Create an instance of an OracleConnection object.
        // The developer must provide the appropriate data source setting.
        var connection = new OracleConnection("User Id=/;Data Source=<oracle>");

        // tnsnames.ora, sqlnet.ora, and cwallet.sso must reside in the same
        // directory as the application executable.  These files can be downloaded
        // from Oracle Cloud for the Oracle Autonomous DB instance.
        connection.TnsAdmin = @".\";

        // Assign the OracleAccessToken to the AccessToken property on the
        // OracleConnection object.
        connection.AccessToken = oracleAccessToken;

        // Open the connection.
        connection.Open();

        // If Open() fails, it will throw an exception.
        Console.WriteLine("Open success.");

        // Dispose the OracleConnection object.
        connection.Dispose();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    // Retrieves an Azure AD access token through the
    // Service Principal Auth flow using a client secret.
    static string GetAccessToken()
    {
      // The developer must configure the Azure AD parameters below.
      string clientId = "<client Id of app registration in Azure AD>";
      string tenantId = "<tenant Id of Azure AD>";
      string clientSecret = "<secret value of app registration in Azure AD>";
      string scope = "<scope of DB registration in Azure AD>";

      // Create a TokenRequestContext object.
      var tokenRequestContext = new TokenRequestContext(new[] { scope });

      // Create a ClientSecretCredential object.
      var credentials = new ClientSecretCredential(tenantId, clientId, clientSecret);

      // Get the access token from Azure AD.
      AccessToken accessToken = credentials.GetToken(tokenRequestContext, default(CancellationToken));

      // Return the access token.
      return accessToken.Token;
    }
  }
}

/* Copyright (c) 2022, Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/
