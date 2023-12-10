// ODP.NET OpenTelemetry Demo
// This sample demonstrates using managed ODP.NET or ODP.NET Core with OpenTelemetry using the HR schema.
// To setup, add NuGet packages: Oracle.ManagedDataAccess.OpenTelemetry, OpenTelemetry, and an OpenTelemetry exporter.
// This sample is configured to use the Console Exporter (OpenTelemetry.Exporter.Console), but can be modified to another exporter.
// Provide the Oracle database password and data source information for the connection string.

using System.Diagnostics;
using OpenTelemetry; // for Sdk
using OpenTelemetry.Trace; // for TracerProvider and TracerProviderBuilder
using Oracle.ManagedDataAccess.Client; // for ODP.NET
using Oracle.ManagedDataAccess.OpenTelemetry; // for ODP.NET OpenTelemetry

class ODP_OTel_Demo
{
    static TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
      .AddOracleDataProviderInstrumentation(o => // ODP.NET OpenTelemetry extension method
      {
        o.EnableConnectionLevelAttributes = true;
        o.RecordException = true;
        o.InstrumentOracleDataReaderRead = true;
        o.SetDbStatementForText = true;
       })
      .AddSource("ODP.NET App")
      .AddConsoleExporter() // OpenTelemetry.Exporter.Console NuGet package extension method
      //.AddZipkinExporter()  // OpenTelemetry.Exporter.Zipkin NuGet package extension method
      .Build()!;

    static ActivitySource activitySource = new ActivitySource("ODP.NET App");
    
    static string conString = @"User Id=hr;Password=<PASSWORD>;Data Source=<NET SERVICE NAME>;";

    static void Main()
    {
        using (OracleConnection con = new OracleConnection(conString))
        {
            using (OracleCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = "select * from employees";

                    // Start OpenTelemetry activity
                    using (Activity activity = activitySource.StartActivity("Retrieve data")!)
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read()) 
                        {
                            // Use query results
                        }
                        reader.Dispose(); 
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

/******************************************************************************
* The MIT License (MIT)
* 
* Copyright (c) 2015, 2023 Oracle
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:

* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.

* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
 *****************************************************************************/
