
/* Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved. */

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

// Demo: Controlling Size of Data to Fetch Per DB Round Trip
// ODP.NET apps can use the FetchSize and RowSize properties
// to set how much data to fetch from the DB per server round
// trip. The FetchSize can be set at run-time and be based on
// the size of the result set row.
//
// Ideally, apps should set FetchSize to the amount of data
// the end user will consume at one time. For example, if
// the end user pages through 20 rows at a time, then the
// FetchSize should be set to 20 * RowSize.
//
// This demo shows the difference between fetching data one
// row at a time vs. fetching all the data at one time.

using System;
using Oracle.ManagedDataAccess.Client;

namespace FetchSize
{
    class Program
    {
        static void Main(string[] args)
        {
            string conString = "User Id=hr;Password=<Password>;Data Source=<ip or hostname>:1521/<service name>;";
            try
            {
                // Create connection and command objects to test performance
                OracleConnection con = new OracleConnection(conString);
                con.Open();

                // Create a command within the context of the connection, provide the SQL statement to execute,
                // and do not use statement cache for fairer comparisons between executions.
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from employees";
                cmd.AddToStatementCache = false;

                // Execute the command and fetch one row at a time
                // FetchSize is set to 1 row on the DataReader
                // Note: RowSize value populated at run-time immediately after statement execution
                DateTime start_time = DateTime.Now;

                for (int i = 0; i < 200; i++)
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader.FetchSize = cmd.RowSize * 1;
                    while (reader.Read())
                        ;
                    reader.Dispose();
                }

                DateTime end_time = DateTime.Now;
                TimeSpan ts = end_time - start_time;
                double ts1 = Math.Round(ts.TotalSeconds, 3);

                Console.WriteLine("Fetch Size = 1: " + ts1 + " seconds");

                // Repeat command executions but now with FetchSize set to 107 rows
                // Note: HR.EMPLOYEES table has 107 rows by default
                start_time = DateTime.Now;

                for (int i = 0; i < 200; i++)
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader.FetchSize = cmd.RowSize * 107;
                    while (reader.Read())
                        ;
                    reader.Dispose();
                }

                end_time = DateTime.Now;
                ts = end_time - start_time;
                double ts2 = Math.Round(ts.TotalSeconds, 3);

                Console.WriteLine("Fetch Size = 100: " + ts2 + " seconds");
                Console.WriteLine();

                Console.WriteLine("Percent difference: " +
                  Math.Round(((ts1 - ts2) / ts2) * 100, 2) + "%");
                Console.WriteLine("Press 'Enter' to continue");
                Console.ReadLine();

                cmd.Dispose();
                con.Dispose();
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
