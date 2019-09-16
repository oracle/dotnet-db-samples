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

// This sample app shows how to use ODP.NET application context and its properties

namespace AppContext
{
    class Program
    {
        static void Main(string[] args)
        {
            using (OracleConnection con = new OracleConnection())
            {

                // Connect string
                con.ConnectionString = "Data Source=<data source>;" +
                    "User Id=hr;Password=<password>;";

                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();

                        // Set application context values after opening the connection
                        con.ClientId = "Alex";
                        con.ActionName = "Retrieve Employee Names";
                        con.ModuleName = "HR .NET App";
                        con.ClientInfo = "Version 1";

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

                        // Before closing connection, view the application context values in the database. 
                        // Execute using SYS user: SELECT ACTION, CLIENT_IDENTIFIER, CLIENT_INFO, MODULE, USERNAME FROM V$SESSION WHERE USERNAME='HR';

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
