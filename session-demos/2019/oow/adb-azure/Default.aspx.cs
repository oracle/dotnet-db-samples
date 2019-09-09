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
    //Demo: Managed ODP.NET Web app that connects to Oracle Autonomous DB.
    // This app connects to Oracle Autonomous DB, retrieves the first  
    // employee in the HR schema, and outputs it to the web page. 
   
    public partial class autonomous_managed_odp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Enter in the user id and password.
            string conString = "User Id=hr;Password=<password>;";

            //Enter net service name for the Oracle Autonomous DB.
            conString += "Data Source=<Oracle net service name>;";

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "SELECT FIRST_NAME, LAST_NAME FROM EMPLOYEES";

                        //Execute the command and retrieve the data.
                        OracleDataReader reader = cmd.ExecuteReader();
                        reader.Read();

                        //Output database version connection info to page.
                        TextBox1.Text = "Employee #1: " + reader.GetString(0) + " " + reader.GetString(1);
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
