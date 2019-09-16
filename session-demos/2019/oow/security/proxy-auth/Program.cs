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
using Oracle.ManagedDataAccess.Client;

// This sample app shows how ODP.NET connects using proxy authentication

// DB setup SQL to execute:
// 1. create user appserver identified by appserver;
// 2. grant connect, resource to appserver;
// 3. alter user hr grant connect through appserver;

// Check on setup with using SYS account: SELECT * FROM PROXY_USERS;

namespace ProxyUser
{
    class Program
    {
        static void Main(string[] args)
        {
            using (OracleConnection con = new OracleConnection())
            {
                // Connecting using proxy authentication
                // Add data souce and HR password
                con.ConnectionString = "Data Source=<data source>;" +
                "User Id=hr;Password=<password>;" +
                "Proxy User Id=appserver;Proxy Password=appserver;";

                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;

                        //Use the command to display employee names from 
                        // the EMPLOYEES table
                        cmd.CommandText = "select first_name from employees where department_id = :id";

                        // Assign id to the department number 20 
                        OracleParameter id = new OracleParameter("id", 20);
                        cmd.Parameters.Add(id);

                        //Execute the command and use DataReader to display the data
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Console.WriteLine("Employee First Name: " + reader.GetString(0));
                        }

                        // Before closing connection, view the sessions created for the real user and proxy user. 
                        // Execute using SYS user: SELECT SID, USERNAME FROM V$SESSION WHERE USERNAME='APPSERVER' OR USERNAME='HR';

                        Console.ReadLine();

                        id.Dispose();
                        reader.Dispose();
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
