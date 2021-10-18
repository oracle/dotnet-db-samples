using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;

namespace ADB_ODPCore_ASPCore3
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
                    //Demo: ODP.NET Core application for ASP.NET Core 3.1 to connect, 
                    //   query, and return results from Oracle Autonomous Database

                    //Set the user id and password			
                    string conString = "User Id=admin;Password=<password>;" +

                    //Set Data Source value to an Oracle net service name in
                    //  the tnsnames.ora file
                    "Data Source=<ADB net service name>;";

                    //Set the directory where the sqlnet.ora, tnsnames.ora, and 
                    //  wallet files are located
                    OracleConfiguration.TnsAdmin = @"<ADB credential files directory>";
                    OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

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
