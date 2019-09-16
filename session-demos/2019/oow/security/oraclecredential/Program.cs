/* Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved. */

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
 
using System;
using System.Security;
using Oracle.ManagedDataAccess.Client;

namespace UsingOracleCredential
{
    class Program
    {
        static void Main(string[] args)
        {
            // This sample app shows how to connect with OracleCredential using the HR schema

            // Connection string without password
            string conString = "Data Source=<data souce>;";

            // Provide the password and make read-only
            // This sample assumes the password is 'hr'
            SecureString secPwd = new SecureString();
            secPwd.AppendChar('h');
            secPwd.AppendChar('r');
            secPwd.MakeReadOnly();

            // Create OracleCredential
            OracleCredential oc = new OracleCredential("hr", secPwd);

            using (OracleConnection con = new OracleConnection(conString, oc))
            {
                con.Open();
                Console.WriteLine("Successfully connected to database");
                Console.ReadLine();
            }
        }
    }
}
