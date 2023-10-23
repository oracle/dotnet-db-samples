using Oracle.ManagedDataAccess.Client;
using System.Threading;
using System;

// This code sample demonstrates using synchronous ODP.NET (managed or core) and times its execution time.
// This sample uses the Oracle HR sample schema.

class ODPNET_Sync
{
    static void Main()
    {
        // Add password and data source to connect to your Oracle database
        string conString = @"User Id=hr;Password=<PASSWORD>;Data Source=<NET SERVICE NAME>;";

        using (OracleConnection con = new OracleConnection(conString))
        {
            // Measure time Open takes before next operation can start execution
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
            // Measure time all the sync operations took
            DateTime end_time_all = DateTime.Now;

            // Calculate connection open time and write result to console
            TimeSpan ts_open = end_time_open - start_time;
            double ts_open1 = Math.Round(ts_open.TotalSeconds, 2);
            Console.WriteLine("Synchronous connection open time: " + ts_open1 + " seconds");

            // Calculate overall operation time and write result to console
            TimeSpan ts_all = end_time_all - start_time;
            double ts_all1 = Math.Round(ts_all.TotalSeconds, 2);
            Console.WriteLine("Synchronous ODP.NET operations time: " + ts_all1 + " seconds");
        }
    }
}

/* Copyright (c) 2023 Oracle and/or its affiliates. All rights reserved.     */

/******************************************************************************
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *   limitations under the License.
 * 
 *****************************************************************************/
