/*
 * This sample program utilizes the OCI .NET SDK to obtain a DB token. 
 * The token is then used to open a connection to an Oracle Database using IAM DB token 
 * authentication via the OracleConnection.Open() method.
 * This sample also shows how to open a connection to an Oracle Database using IAM DB token 
 * authentication via the OracleConnection.OpenWithNewToken() method using a new/refreshed 
 * DB token before the initially obtained token expires.
 * 
 * As a prerequisite, your cloud administrator must enable OCI IAM Single Sign-on with ADB.
 * 
 * To run this sample, set the Oracle data source in the code below. If necessary, provide 
 * the TNS Admin directory location and wallet location. Set the OCI configuration profile
 * if you are not using DEFAULT as its value.
 *
 * Requirements:
 * .NET 5 runtime or later is required.
 * 
 * Add the following NuGet packages to your .NET project:
 * 1. OCI .NET SDK Version >= 29.4
 *    Packages: 
 *      OCI.DotNetSDK.Identitydataplane
 *          Includes dependency OCI.DotNetSDK.Common
 *      
 * 2. ODP.NET Core version >= 3.21.41
 *    Packages:
 *      Oracle.ManagedDataAccess.Core
 *      
 * 3. Portable.BouncyCastle >= 1.9.0
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Oci.Common.Auth;
using Oci.IdentitydataplaneService;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace IAM_OpenWithNewToken
{
    class Program_bak
    {
        public static OracleAccessToken m_accessToken;
        public static bool m_useOpenWithNewToken = false;

        public static void Main()
        {
            // Add the ADB instance connection info, such as net service name or TNS descriptor
            string dataSource = @"<ORACLE DATA SOURCE>";
            
            // Add the ADB tnsnames.ora and/or sqlnet.ora directory location here
            // Not required if using walletless TLS
            OracleConfiguration.TnsAdmin = @"<DIRECTORY NAME>";

            // Add the ADB wallet directory location here
            // Not required if using walletless TLS
            OracleConfiguration.WalletLocation = @"<DIRECTORY NAME>";

            // Later on, we will create an authentication provider that will use an OCI profile.
            // Provide the profile name. We have used "DEFAULT" as the profile name in this
            // sample. The configuration file is located at %HOMEDRIVE%%HOMEPATH%\.oci\config on 
            // Windows. On other operating systems, the location is ~/.oci/config
            // Modify the profile name below if you use a different name.
            // Refer to https://docs.cloud.oracle.com/en-us/iaas/Content/API/Concepts/sdkconfig.htm#SDK_and_CLI_Configuration_File to prepare a configuration file. 
            string profile = "DEFAULT";

            // The public key that is needed for getting the DB token
            string publicKey = null;

            // The private key that is needed for creating the OracleAccessToken
            char[] privateKey = null;

            // The DB Token that is needed for creating the OracleAccessToken
            string dbToken = null;

            // Generate the pair of private and public key
            GenerateKeyPair(out publicKey, out privateKey);

            // Obtain the DB token using OCI .NET SDK
            GetDBToken(publicKey, out dbToken, profile);

            // Create an OracleAccessToken
            // Note: For security reasons, CreateAccessToken() clears out the passed dbToken
            // and privatekey char[]s after storing them in a more secure format
            CreateAccessToken(dbToken, privateKey);

            // Create a connection using IAM authentication and execute a SQL SELECT
            DoWork(dataSource, dbToken, privateKey);

            // It is recommended that OpenWithNewToken method only be used in cases if the
            // application is unable to or fails to provide the refreshed/updated DB token
            // and private key through the token refresh call back for some reason.
            // For this sample, we want to forcibly open a connection using
            // OpenWithNewToken to show its use case without waiting for 1 hour.
            // So, the function below that extracts the DB token expiration time and waits
            // until 60 seconds before its expiration is commented out.
            //WaitForTokenExpiration(dbToken);

            // Generate another pair of private and public key to be used for OpenWithNewToken
            GenerateKeyPair(out publicKey, out privateKey);

            // Refresh/update the DB token from IAM
            // Application does NOT need to get a new/refreshed db access token unless the
            // previous DB token is about to expire
            GetDBToken(publicKey, out dbToken, profile);

            // Set this to true for the connection to be opened using OpenWithNewToken
            m_useOpenWithNewToken = true;

            // Create a connection using IAM authentication (with a refreshed token) and
            // execute a SQL SELECT
            DoWork(dataSource, dbToken, privateKey);
        }

        public static void WaitForTokenExpiration(string dbToken)
        {
            // Getting DB token expiry time
            OracleAccessToken.ParseDBToken(dbToken.ToCharArray(), out string subUserInNewDBToken, out DateTimeOffset dbTokenExpTime, out string jwkValInNewDBToken);

            // Get current time
            DateTimeOffset currentTime = DateTimeOffset.Now.ToUniversalTime();

            // Calculate time difference between DB expiry time and current time
            TimeSpan timeDiff = TimeSpan.Zero;
            if (dbTokenExpTime > currentTime)
                timeDiff = dbTokenExpTime - currentTime;

            // Convert time difference into milliseconds
            int waitTime = (int)timeDiff.TotalMilliseconds;

            // Wait until 60 seconds before the DB Token expires
            waitTime -= 60000;
            if (waitTime > 0)
                Thread.Sleep(waitTime);
        }

        public static void DoWork(string dataSource, string dbToken, char[] privateKey)
        {
            try
            {
                // The connection string to use DB tokens has a User Id set to "/" and
                // an empty password.
                string connection_string = "user id=/; data source=" + dataSource;

                // Create a connection object
                OracleConnection con = new OracleConnection(connection_string);

                // Get the current user names associated with the connection object
                GetCurrentUserName(con, dbToken, privateKey);

                // Dispose the connection object
                con.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void GetCurrentUserName(OracleConnection con, string dbToken, char[] privateKey)
        {
            // Provide the access token
            con.AccessToken = m_accessToken;

            // Open the connection
            if (!m_useOpenWithNewToken)
            {
                Console.Write("Using Open() to connect using IAM Authentication...");
                con.Open();
                Console.WriteLine("Successful!");
            }
            else
            {
                Console.Write("Using OpenWithNewToken() to connect using IAM Authentication...");

                // Note: OpenWithNewToken() call clears out the passed DB token and
                // privateKey char[]s after storing them in a more secure format
                con.OpenWithNewToken(dbToken.ToCharArray(), privateKey);
                Console.WriteLine("Successful!");
            }

            // Create a command object
            OracleCommand cmd = con.CreateCommand();

            // Provide the SQL query to execute
            cmd.CommandText = "select user from dual";

            // Output the user name obtained from the SQL query
            Console.WriteLine(cmd.ExecuteScalar());

            // Dispose the command object
            cmd.Dispose();
        }

        public static void CreateAccessToken(string dbToken, char[] privateKey)
        {
            try
            {
                // Create a new OracleAccessToken
                m_accessToken = new OracleAccessToken(dbToken.ToCharArray(), privateKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void GenerateKeyPair(out string publicKey, out char[] privateKey)
        {
            try
            {
                // Construct the RSACryptoServiceProvider object
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

                // Extract the public/private key pair
                var rsaKeyPair = DotNetUtilities.GetRsaKeyPair(rsa);

                // Extract the private key
                var pkcs8Gen = new Pkcs8Generator(rsaKeyPair.Private);
                var pemObj = pkcs8Gen.Generate();

                MemoryStream memoryStream = new MemoryStream();
                TextWriter streamWriter = new StreamWriter(memoryStream);
                PemWriter pemWriter = new PemWriter(streamWriter);
                pemWriter.WriteObject(pemObj);
                streamWriter.Flush();

                // Extract byte array from memory stream
                byte[] bytearray = memoryStream.GetBuffer();

                // Convert byte array into char array
                privateKey = Encoding.ASCII.GetChars(bytearray);

                // Clear byte array
                Array.Clear(bytearray, 0, bytearray.Length);

                // Dispose stream writer and memory stream
                streamWriter.Dispose();
                memoryStream.Dispose();

                // Extract the public key
                TextWriter stringWriter = new StringWriter();
                pemWriter = new PemWriter(stringWriter);
                pemWriter.WriteObject(rsaKeyPair.Public);
                stringWriter.Flush();
                publicKey = stringWriter.ToString();

                // Display the extracted public key
                Console.WriteLine("public key, {0}", publicKey);

                // Display the extracted private key
                foreach (char ch in privateKey)
                {
                    Console.Write(ch);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string GetDBToken(string publicKey, out string dbToken, in string profile)
        {
            try
            {
                // Create a dependent object with scope and public key information
                var generateScopedAccessTokenDetails = new Oci.IdentitydataplaneService.Models.GenerateScopedAccessTokenDetails
                {
                    Scope = "urn:oracle:db::id::*",
                    PublicKey = publicKey
                };

                // Create a request using the dependent object
                var generateScopedAccessTokenRequest = new Oci.IdentitydataplaneService.Requests.GenerateScopedAccessTokenRequest
                {
                    GenerateScopedAccessTokenDetails = generateScopedAccessTokenDetails
                };

                // Create authentication provider using designated OCI configuration profile
                var provider = new ConfigFileAuthenticationDetailsProvider(profile);

                // Create a service client and send the request
                using (var client = new DataplaneClient(provider))
                {
                    // Request a DB token
                    var response = client.GenerateScopedAccessToken(generateScopedAccessTokenRequest).Result;

                    // Retrieve value from the response
                    dbToken = response.SecurityToken.Token;

                    // Display retrived DB token
                    Console.WriteLine("-----BEGIN Db Token-----\n{0}\n-----END Db Token-----", dbToken);

                    return dbToken;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
