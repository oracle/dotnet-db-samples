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
  class OnsServersExample
  {
    static void Main(string[] args)
    {
      // Example to configure ONS Servers for the ODP.NET Core provider.

      // Add server through Add method on OracleOnsServerCollection
      OracleConfiguration.OracleOnsServers.Add("db1", "nodeList=host1:port1, host2:port2, host3:port3");

      // Add server through indexer method on OracleOnsServerCollection
      OracleConfiguration.OracleOnsServers["db2"] = "nodeList=m1:p1, m2:p2";

      // Get number of servers configured
      int numServers = OracleConfiguration.OracleOnsServers.Count;

      // Get OracleOnsServerCollection object
      OracleOnsServerCollection serverColl = OracleConfiguration.OracleOnsServers;

      // Add server through Add method on OracleOnsServerCollection
      serverColl.Add("db3", "nodeList=host1:port1, host2:port2, host3:port3");

      // Add server through indexer method on OracleOnsServerCollection
      serverColl["db4"] = "nodeList=m1:p1, m2:p2";

      // Remove a server
      OracleConfiguration.OracleOnsServers.Remove("db2");

      // Get number of servers configured
      numServers = OracleConfiguration.OracleOnsServers.Count;

      // Get value corresponding to a server.
      string serverVal = OracleConfiguration.OracleOnsServers["db1"];

      OracleConnection orclCon = null;

      try
      {
        // Open a test connection
        orclCon = new OracleConnection("user id=hr; password=<password>; data source=oracle");

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
