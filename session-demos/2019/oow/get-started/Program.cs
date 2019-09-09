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
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace GetStartedODPNETCore
{
    class Program
    {
        static void Main(string[] args)
        {

            //Demo: Basic ODP.NET Core application to connect, query, and return
            // results from an OracleDataReader to a console

            //Enter user name and password			
            string conString = "User Id=hr;Password=<password>;" +
            
            //Connect to an Oracle DB with Easy Connect (Plus) or a net service name
            "Data Source=<Easy Connect (Plus) or net service name>;";

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;

                        //Use the command to display employee names from the EMPLOYEES table
                        cmd.CommandText = "select first_name from employees where department_id = :id";

                        //Assign id to the department number 20 
                        OracleParameter id = new OracleParameter("id", 20);
                        cmd.Parameters.Add(id);

                        //Execute the command and use DataReader to display the data
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Console.WriteLine("Employee First Name: " + reader.GetString(0));
                        }

                        id.Dispose();
                        reader.Dispose();
                        Console.WriteLine();

                        //Demo: Batch SQL and REF Cursors
                        // Anonymous PL/SQL block embedded in code - executes in one DB round trip

                        //Reset OracleCommand for use in next demo
                        cmd.Parameters.Clear();
                        cmd.BindByName = false;

                        cmd.CommandText = "DECLARE a NUMBER:= 20; " +
                          "BEGIN " +
                          "OPEN :1 for select first_name,department_id from employees where department_id = 10; " +
                          "OPEN :2 for select first_name,department_id from employees where department_id = a; " +
                          "OPEN :3 for select first_name,department_id from employees where department_id = 30; " +
                          "END;";

                        cmd.CommandType = CommandType.Text;

                        //ODP.NET has native Oracle data types, such as Oracle REF 
                        // Cursors, which can be mapped to .NET data types

                        //Bind REF Cursor Parameters for each department
                        //Select employees in department 10
                        OracleParameter p1 = cmd.Parameters.Add("refcursor1",
                          OracleDbType.RefCursor);
                        p1.Direction = ParameterDirection.Output;

                        //Select employees in department 20
                        OracleParameter p2 = cmd.Parameters.Add("refcursor2",
                          OracleDbType.RefCursor);
                        p2.Direction = ParameterDirection.Output;

                        //Select employees in department 30
                        OracleParameter p3 = cmd.Parameters.Add("refcursor3",
                          OracleDbType.RefCursor);
                        p3.Direction = ParameterDirection.Output;

                        //Execute batched statement
                        cmd.ExecuteNonQuery();

                        //Let's retrieve the three result sets with DataReaders
                        OracleDataReader dr1 =
                          ((OracleRefCursor)cmd.Parameters[0].Value).GetDataReader();
                        OracleDataReader dr2 =
                          ((OracleRefCursor)cmd.Parameters[1].Value).GetDataReader();
                        OracleDataReader dr3 =
                          ((OracleRefCursor)cmd.Parameters[2].Value).GetDataReader();

                        //Let's retrieve the results from the DataReaders
                        while (dr1.Read())
                        {
                            Console.WriteLine("Employee Name: " + dr1.GetString(0) + ", " +
                              "Employee Dept:" + dr1.GetDecimal(1));
                        }
                        Console.WriteLine();

                        while (dr2.Read())
                        {
                            Console.WriteLine("Employee Name: " + dr2.GetString(0) + ", " +
                              "Employee Dept:" + dr2.GetDecimal(1));
                        }
                        Console.WriteLine();

                        while (dr3.Read())
                        {
                            Console.WriteLine("Employee Name: " + dr3.GetString(0) + ", " +
                              "Employee Dept:" + dr3.GetDecimal(1));
                        }

                        //Clean up
                        p1.Dispose();
                        p2.Dispose();
                        p3.Dispose();
                        dr1.Dispose();
                        dr2.Dispose();
                        dr3.Dispose();

                        Console.WriteLine("Press 'Enter' to continue");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Console.ReadLine();
                }
            }
        }
    }
}
