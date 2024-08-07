using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;
using System.Threading;
using System;

// This code sample demonstrates using synchronous ODP.NET (managed or core) without pipelining.
// It times opening a connection and several query executions.
// To run this app, add your database's HR schema User Id, Password, and Data Source values
// with ODP.NET 23ai or higher connecting to an Oracle Database 23ai or higher.

class ODPNET_Sync_And_No_Pipelining
{
    static void Main()
    {
        // Add password and data source to connect to your Oracle database
        string conString = "User Id=hr;Password=<PASSWORD>;Data Source=<DATA SOURCE>;";
        
        using (OracleConnection con = new OracleConnection(conString))
        {
            string cmdText1 = "SELECT * FROM EMPLOYEES";
            string cmdText2 = "SELECT * FROM DEPARTMENTS";
            string cmdText3 = "SELECT * FROM JOBS";

            OracleCommand cmd1 = new OracleCommand(cmdText1, con);
            OracleCommand cmd2 = new OracleCommand(cmdText2, con);
            OracleCommand cmd3 = new OracleCommand(cmdText3, con);

            // Measure how long connection open takes
            DateTime start_time = DateTime.Now;
            con.Open();
            DateTime end_time_open = DateTime.Now;

            // Simulate an operation that takes one second
            Thread.Sleep(1000);

            // Measure time for query executions
            DateTime start_time_query = DateTime.Now;
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            cmd3.ExecuteNonQuery();

            // Simulate an operation that takes one second
            Thread.Sleep(1000);

            // Record time all the synchronous operations took plus the sleep time
            DateTime end_time_all = DateTime.Now;

            // Calculate connection open time
            TimeSpan ts_open = end_time_open - start_time;
            double ts_open1 = Math.Round(ts_open.TotalSeconds, 2);
            Console.WriteLine("Synchronous connection open time: " + ts_open1 + " seconds");

            // Calculate SQL executions time
            TimeSpan ts_sql = end_time_all - start_time_query;
            double ts_sql1 = Math.Round(ts_sql.TotalSeconds, 2);
            Console.WriteLine("Synchronous query execution time: " + ts_sql1 + " seconds");

            // Calculate overall ODP.NET operation time
            TimeSpan ts_all = end_time_all - start_time;
            double ts_all1 = Math.Round(ts_all.TotalSeconds, 2);
            Console.WriteLine("Synchronous operation total time: " + ts_all1 + " seconds");

            cmd1.Dispose();
            cmd2.Dispose();
            cmd3.Dispose();
        }
    }
}

/* Copyright (c) 2024 Oracle and/or its affiliates. All rights reserved. */
 
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
