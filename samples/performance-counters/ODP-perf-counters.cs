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

// This sample demonstrates how to programmatically use ODP.NET performance counters.
// The app will exceed the maximum limit of connections allowed in the pool.
// The exception handler will retrieve and display the number of pooled connections.

using System;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

class Diagnostics
{
    static void Main()
    {

        // Enter your password and data source information.
        // Pool size is set to a maximum of 5 connections.
        string constr = "user id=hr;password=<password>;max pool size=5;data source=localhost:1521/<service name>;";
        try
        {
            OracleConnection[] cons = new OracleConnection[6];

            // Creates 6 connections when maximum pool size set to 5
            for (int i = 0; i <= 5; i++) 
            {
                Console.Write("Creating connection # {0}...", i+1);
                cons[i] = new OracleConnection(constr);
                cons[i].Open();
                Console.WriteLine("Connected!");
            }
        }
        catch (Exception ex)
        {
            // Outputs application error message
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine();

            // Retrieves performance counter metric for number of pooled connections and display the result
            // Replace the executable name below with the name of your executable, such as "ConsoleApp2.exe[{0},1]"
            string instanceName = string.Format("ConsoleApp1.exe[{0},1]", Process.GetCurrentProcess().Id);
            PerformanceCounter pc = new PerformanceCounter("ODP.NET, Managed Driver", "NumberOfPooledConnections", instanceName);
            Console.WriteLine("Current number of pooled connections is: {0}", pc.NextValue());
            Console.ReadLine();
        }
    }
}
