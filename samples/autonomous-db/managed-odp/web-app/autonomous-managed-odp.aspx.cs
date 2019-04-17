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

namespace Autonomous_Managed_ODP_WebApp
{
    //Demo: Managed ODP.NET Web application that connects to Oracle Autonomous DB.
    // This application connects to Oracle Autonomous DB, retrieves the database version information, and
    // outputs it to the web page. Any database user with read and connect privileges can use this 
    // demo code.
    public partial class autonomous_managed_odp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Enter in the user id and password.
            string conString = "User Id=<USER ID>;Password=<PASSWORD>;";

            //Enter port, host name or IP, service name, and wallet directory for your Oracle Autonomous DB.
            conString += "Data Source=(description=(address=(protocol=tcps)(port=<PORT>)(host=<HOSTNAME OR IP>))(connect_data=(service_name=<SERVICE NAME>))(SECURITY = (MY_WALLET_DIRECTORY = <DIRECTORY LOCATION>)));";

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "SELECT * FROM v$version";

                        //Execute the command and use DataReader to retrieve the data.
                        OracleDataReader reader = cmd.ExecuteReader();
                        reader.Read();

                        //Output database version connection info to page.
                        TextBox1.Text = "Connected to " + reader.GetString(0);
                    }
                    catch (Exception ex)
                    {
                        //If application fails, output error message to page.
                        string text = ex.Message;
                        TextBox1.Text = text;
                    }
                }
            }
        }
    }
}
