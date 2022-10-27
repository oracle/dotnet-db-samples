using Oracle.ManagedDataAccess.Client;

// This app measures how long it takes synchronous ODP.NET operations.
class ODPNET_Sync
{
    static void Main()
    {
        // Modify User Id, Password, and Data Source as needed to connect
        string conString = "User Id=hr;Password=<PASSWORD>;Data Source=<DATA SOURCE>;";

        using (OracleConnection con = new OracleConnection(conString))
        {
            DateTime start_time = DateTime.Now;
            con.Open();
            DateTime end_time_open = DateTime.Now;

            // Simulate operation that takes one second
            Thread.Sleep(1000);

            string cmdText = "SELECT * FROM EMPLOYEES FETCH FIRST 1 ROWS ONLY";
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

            // Calculate overall operation time
            TimeSpan ts_all = end_time_all - start_time;
            double ts_all1 = Math.Round(ts_all.TotalSeconds, 2);
            Console.WriteLine("Synchronous ODP.NET operations time: " + ts_all1 + " seconds");
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
