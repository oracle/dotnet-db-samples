using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;

namespace ODPCoreASPCore3
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    //Demo: Basic ODP.NET Core application for ASP.NET Core 3.1
                    // to connect, query, and return results to a web page

                    //Set the user id and password			
                    string conString = "User Id=hr;Password=<password>;" +

                    //Set Data Source value to an Oracle net service name in
                    //  the tnsnames.ora file or use EZ Connect
                    "Data Source=<ip or hostname>:1521/<service name>;";

                    using (OracleConnection con = new OracleConnection(conString))
                    {
                        using (OracleCommand cmd = con.CreateCommand())
                        {
                            try
                            {
                                con.Open();
                                cmd.BindByName = true;

                                //Use the command to display employee names from 
                                // the EMPLOYEES table
                                cmd.CommandText = "select first_name, last_name from employees where department_id = :id";

                                // Assign id to the department number 50 
                                OracleParameter id = new OracleParameter("id", 50);
                                cmd.Parameters.Add(id);

                                //Execute the command and use DataReader to display the data
                                OracleDataReader reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    await context.Response.WriteAsync("Employee Name: " + reader.GetString(0) + " " + reader.GetString(1) + "\n");
                                }

                                reader.Dispose();
                            }
                            catch (Exception ex)
                            {
                                await context.Response.WriteAsync(ex.Message);
                            }
                        }
                    }
                });
            });
        }
    }
}

/* Copyright (c) 2020, Oracle and/or its affiliates. All rights reserved. */

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
