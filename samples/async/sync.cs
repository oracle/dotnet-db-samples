using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;
using System.Threading;
using System;

// This app uses synchronous ODP.NET (managed or core) APIs to open a connection,
// execute a SQL statement, and read the results. It times how long these operations take.
// To run this app, add your database's HR schema User Id, Password, and Data Source values
// with ODP.NET 23ai or higher connecting to an Oracle Database 19c or higher.


class ODPNET_Sync
{
    static void Main()
    {
        // Add password and data source to connect to your Oracle database
        string conString = "User Id=hr;Password=<PASSWORD>;Data Source=<DATA SOURCE>;";

        using (OracleConnection con = new OracleConnection(conString))
        {            
            //Time how long it takes to open a connection
            DateTime start_time = DateTime.Now;
            con.Open();
            DateTime end_time_open = DateTime.Now;

            // Simulate operation that takes one second
            Thread.Sleep(1000);

            string cmdText = "SELECT * FROM EMPLOYEES FETCH FIRST 100 ROWS ONLY";
            using (OracleCommand cmd = new OracleCommand(cmdText, con))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                }
            }
            DateTime end_time_all = DateTime.Now;

            // Calculate connection open time
            TimeSpan ts_open = end_time_open - start_time;
            double ts_open1 = Math.Round(ts_open.TotalSeconds, 2);
            Console.WriteLine("Synchronous connection open time: " + ts_open1 + " seconds");

            // Calculate overall ODP.NET operation time
            TimeSpan ts_all = end_time_all - start_time;
            double ts_all1 = Math.Round(ts_all.TotalSeconds, 2);
            Console.WriteLine("Synchronous ODP.NET overall time: " + ts_all1 + " seconds");
        }
    }
}

/* Copyright (c) 2023, 2024 Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/
