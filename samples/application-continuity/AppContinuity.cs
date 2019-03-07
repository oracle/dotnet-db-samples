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
using System.Threading;
using System.Transactions;
using Oracle.DataAccess.Client;

namespace AppContinuity
{
    class Program
    {
        static void Main(string[] args)
        {

            //Demo: ODP.NET Application Continuity
            //Demonstrates that applications can recover and continue after 
            // recoverable errors without end user disruption. Recoverable errors
            // are errors that arise due to an external system failure, 
            // independent of the application session logic that is executing. 
            // Recoverable errors occur following planned and unplanned outages 
            // of foregrounds, networks, nodes, storage, and databases, such as a 
            // network outage, instance failure, hardware failure, network failure, 
            // configuration change, or patching.

            //To use Application Continuity with ODP.NET, set 
            // Application Continuity=true (default) in the connection string.

            //Setup instructions:
            // A) On the database server :
            //  1) Setup HR schema by using script %ORACLE_HOME%\demo\schema\human_resources\hr_main.sql 
            //      if not already available.
            //  2) Run "GRANT execute on DBMS_APP_CONT to HR;" so that ODP.NET can determine the 
            //      in-flight transaction status following a recoverable error.
            // B) Modify the following connection attributes in this sample code:
            //  1) Password: Password you specified while setting up HR schema setup.
            //  2) Data Source: Connection descriptor or TNS alias to connect to the database.

            //Runtime instructions:
            // You can intermittently execute the following command on the database
            // to simulate a recoverable error condition and observe AC in action:
            // "Execute dbms_service.disconnect_session('your service', dbms_service.immediate );"
            //
            // With Application Continuity enabled, the end user will observe 
            // no error message, only a slight delayed execution.

            //Provide user id and password for Oracle user	
            string conString = "User Id=hr;Password=<password>;" +

            //Provide Oracle data source info.
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<hostname or IP>)(PORT=<port>))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=<service name>)));";

            //Loop infinitely
            while (true)
            {
                try
                {
                    using (OracleConnection con = new OracleConnection(conString))
                    {
                        using (OracleCommand cmd = con.CreateCommand())
                        {
                            //Start transaction
                            using (TransactionScope scope = new TransactionScope())
                            {
                                con.Open();
                                cmd.CommandText = "update employees set salary=salary+1 where employee_id = 100";

                                cmd.ExecuteNonQuery();

                                //Commit
                                scope.Complete();

                                Console.WriteLine("Salary incremented.");
                            }
                            //Sleep for 1 second
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
