
/* Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved. */

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

namespace NetCoreApp
{
  class DataSourcesExample
  {
    static void Main(string[] args)
    {
      // Example to configure Data Sources for the ODP.NET Core provider.

      // Add data source through Add method on OracleDataSourceCollection
      OracleConfiguration.OracleDataSources.Add("orcl1", "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<hostname>)(PORT=1234))(CONNECT_DATA=(SERVICE_NAME=<service name>)(SERVER=dedicated)))");

      // Add data source through indexer method on OracleDataSourceCollection
      OracleConfiguration.OracleDataSources["orcl2"] = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<hostname>)(PORT=1234))(CONNECT_DATA=(SERVICE_NAME=<service name>)(SERVER=dedicated)))";

      // Get number of data sources configured
      int numDataSources = OracleConfiguration.OracleDataSources.Count;

      // Get OracleDataSourceCollection object
      OracleDataSourceCollection dsColl = OracleConfiguration.OracleDataSources;

      // Add server through Add method on OracleDataSourceCollection
      dsColl.Add("orcl3", "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<hostname>)(PORT=1234))(CONNECT_DATA=(SERVICE_NAME=<service name>)(SERVER=dedicated)))");

      // Add server through indexer method on OracleDataSourceCollection
      dsColl["orcl4"] = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<hostname>)(PORT=1234))(CONNECT_DATA=(SERVICE_NAME=<service name>)(SERVER=dedicated)))";

      // Remove a data source
      OracleConfiguration.OracleDataSources.Remove("db2");

      // Get number of data sources configured
      numDataSources = OracleConfiguration.OracleDataSources.Count;

      // Get value corresponding to a data source.
      string dsVal = OracleConfiguration.OracleDataSources["db1"];

      OracleConnection orclCon = null;

      try
      {
        // Open a test connection
        orclCon = new OracleConnection("user id=hr; password=<password>; data source=orcl3");

        orclCon.Open();
        orclCon.Close();
      }
      catch (OracleException ex)
      {
        Console.WriteLine(ex);
      }
      finally
      {
        // Close the connection
        if (null != orclCon)
          orclCon.Close();
      }
    }
  }
}
