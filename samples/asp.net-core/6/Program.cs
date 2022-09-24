using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (context) =>
{
    //Set the user id, password and data source
    //Set Data Source value to Oracle connect descriptor or net service name
    string conString = "User Id=HR;Password=<PASSWORD>;Data Source=<DATA SOURCE>;";
    using (OracleConnection con = new OracleConnection(conString))
    {
        using (OracleCommand cmd = con.CreateCommand())
        {
            try
            {
                con.Open();

                //Use the command to display employee names from EMPLOYEES table
                cmd.CommandText = "select first_name, last_name from employees where department_id = :id";

                // Assign id to the department number 50 
                cmd.BindByName = true;
                OracleParameter id = new OracleParameter("id", 50);
                cmd.Parameters.Add(id);

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    await context.Response.WriteAsync("Employee Name: " + reader.GetString(0) + " " + reader.GetString(1) + "\n");

                id.Dispose();
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
