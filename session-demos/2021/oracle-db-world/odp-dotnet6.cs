using System;
using Oracle.ManagedDataAccess.Client;

namespace DotNet6_ODP.NET_Demo
{
    class Program
    {
        static void Main(string[] args)
        {

	    Console.WriteLine();
    	    Console.WriteLine("This app is using .NET version: {0}", Environment.Version.ToString());
	    Console.WriteLine();

            //Demo: ODP.NET Core application that connects to Oracle Autonomous DB

            //Enter user id and password, such as ADMIN user	
            string conString = "User Id=<USER>;Password=<PASSWORD>;" +

            //Enter net service name, EZ Connect, or TNS connection string for data source value
            "Data Source=<DATA SOURCE>;";

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        //Uncomment and enter directory the tnsnames.ora and sqlnet.ora files are located, if applicable
                        //OracleConfiguration.TnsAdmin = @"<DIRECTORY>";                 
                        //Uncomment and enter directory where wallet is stored locally, if applicable
                        //OracleConfiguration.WalletLocation =  @"<DIRECTORY>";

                        con.Open();

                        Console.WriteLine("Successfully connected to Oracle Autonomous Database");

                        //Retrieve database version info
                        cmd.CommandText = "SELECT BANNER FROM V$VERSION";
                        OracleDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        Console.WriteLine("The version is " + reader.GetString(0));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }
    }
}

/* Copyright (c) 2021, Oracle and/or its affiliates. All rights reserved. */

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
