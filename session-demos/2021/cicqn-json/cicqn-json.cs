using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ODP.NET_CICQN_JSON_21c
{
    //This app demonstrates how to use Client Initiated Continquous Query Notification (CICQN) and
    // JSON (OSON) data type with Oracle Database 21c. Works with autonomous, cloud, or on-premises DB.

    //Setup steps:
    //1) "GRANT CHANGE NOTIFICATION TO <USER>" to use CICQN
    //2) Create the J_PURCHASEORDER table and insert a JSON document into it. See SQL script.
    //3) Use ODP.NET 21c and Oracle Database 21c to run the console app code.
    //4) Populate the Constants class below with your app-specific values and run.

    class Program
    {
        static class Constants
        {
            //Enter your user id, password, and DB info, such as net service name, 
            // Easy Connect Plus, or connect descriptor
            public const string ConString = "User Id=<USER>;Password=<PASSWORD>;Data Source=<NET SERVICE NAME>;Connection Timeout=30;";

            //Enter directories where your *.ora files and your wallet are located, if applicable
            public const string TnsDir = @"<DIRECTORY WITH *.ORA FILES, IF APPLICABLE>";
            public const string WalletDir = @"<WALLET DIRECTORY, IF APPLICABLE>";

            //Set the query to execute and JSON column to retrieve
            public const string Query = "select PO_DOCUMENT from J_PURCHASEORDER";
            public const string JsonColumn = "PO_DOCUMENT";
        }

        static void Main()
        {
            //Directory where your cloud or database credentials are located, if applicable
            OracleConfiguration.TnsAdmin = Constants.TnsDir;
            OracleConfiguration.WalletLocation = Constants.WalletDir;

            //Turn on Client Initiated Continuous Query Notification
            OracleConfiguration.UseClientInitiatedCQN = true;

            using (OracleConnection con = new OracleConnection(Constants.ConString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        Console.WriteLine("Successfully connected to Oracle Database");
                        Console.WriteLine();

                        //CQN setup
                        OracleDependency dep = new OracleDependency(cmd);
                        cmd.Notification.IsNotifiedOnce = false;
                        dep.OnChange += new OnChangeEventHandler(JSON_Notification);

                        //Retrieve purchase order stored as JSON (OSON) type
                        //Store JSON data as string in disconnected DataSet
                        cmd.CommandText = Constants.Query;
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            foreach (DataTable table in ds.Tables)
                            {
                                foreach (DataRow row in table.Rows)
                                {
                                    //Output the query result to the console
                                    Console.WriteLine(row[Constants.JsonColumn].ToString());
                                }
                            }

                            //While result set is disconnected from database, modify the purchase 
                            // order data through ODT or SQL*Plus, such as with the following SQL:
                            //UPDATE j_purchaseorder SET po_document = json_transform(po_document, SET '$.LastUpdated' = SYSDATE);
                            //Changing the data on database triggers the CICQN event handler.
                            //Each time the update statement runs, a new notification will be sent to the app.
                            
                            Console.ReadLine();
                        }
                        da.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public static void JSON_Notification(object src, OracleNotificationEventArgs args)
        {
            //Each time event handler launches, it retrieves the updated purchase order details
            //Note that the LastUpdated entry has been changed.
            Console.WriteLine();
            Console.WriteLine("Change detected.");

            using (OracleConnection con = new OracleConnection(Constants.ConString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = Constants.Query;
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            foreach (DataTable table in ds.Tables)
                            {
                                foreach (DataRow row in table.Rows)
                                {
                                    Console.WriteLine(row[Constants.JsonColumn].ToString());
                                }
                            }
                        }
                        da.Dispose();
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

/* Copyright (c) 2021, Oracle and/or its affiliates. All rights reserved. */
 
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
