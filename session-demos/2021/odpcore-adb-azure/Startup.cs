using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Oracle.ManagedDataAccess.Client;

namespace <ADD NAMESPACE>
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                    //Demo: ODP.NET Core application for ASP.NET Core 5 to connect, 
                    //  query, and return results from Oracle Autonomous Database

                    //Set the user id and password			
                    string conString = "User Id=ADMIN;Password=<ADD PASSWORD>;" +

                    //Set Data Source value to an Oracle net service name in
                    //  the tnsnames.ora file
                    "Data Source=<ADD DATA SOURCE>;Connection Timeout=30;";

                    //Set the directory where the sqlnet.ora, tnsnames.ora, and 
                    //  wallet files are located
                    OracleConfiguration.TnsAdmin = @"<ADD TNSADMIN and WALLET DIRECTORY>";
                    OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

                    using (OracleConnection con = new OracleConnection(conString))
                    {
                        using (OracleCommand cmd = con.CreateCommand())
                        {
                            try
                            {
                                con.Open();
                                await context.Response.WriteAsync("Connected to Oracle Autonomous Database." + "\n" + "\n");

                                //Retrieve first 20 customer names, cities, and credit limits from Sales History (SH) schema.
                                //SH schema is available as a read-only data set in shared ADB
                                cmd.CommandText = "select CUST_FIRST_NAME, CUST_LAST_NAME, CUST_CITY, CUST_CREDIT_LIMIT " +
                                    "from SH.CUSTOMERS order by CUST_ID fetch first 20 rows only";
                                OracleDataReader reader = cmd.ExecuteReader();

                                while (reader.Read())
                                    await context.Response.WriteAsync(reader.GetString(0) + " " + reader.GetString(1) + " in "
                                        + reader.GetString(2) + " has " + reader.GetInt16(3) + " in credit." + "\n");

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
