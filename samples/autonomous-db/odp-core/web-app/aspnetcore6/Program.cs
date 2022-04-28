using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run(async context =>
{
    //Set the user id and password			
    string conString = "User Id=ADMIN;Password=<PASSWORD>;Connection Timeout=180;" +

    //Set Data Source value to an Oracle connect descriptor or an Oracle net service name
        "Data Source=<CONNECT DESCRIPTOR OR NET SERVICE NAME>;";

    //If using Oracle client configuration files, uncomment line below and set the relative
    // subdirectory of the website root directory where the files are located.
    //OracleConfiguration.TnsAdmin = $".{Path.DirectorySeparatorChar}<SUBDIRECTORY NAME>";

    using (OracleConnection con = new OracleConnection(conString))
    {
        using (OracleCommand cmd = con.CreateCommand())
        {
            try
            {
                con.Open();
                await context.Response.WriteAsync("Connected to Oracle Autonomous Database." + "\n");

                //Retrieve database version info
                cmd.CommandText = "SELECT BANNER FROM V$VERSION";
                OracleDataReader reader = cmd.ExecuteReader();
                reader.Read();
                await context.Response.WriteAsync("The version is " + reader.GetString(0));

                reader.Dispose();
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
});

app.Run();

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
