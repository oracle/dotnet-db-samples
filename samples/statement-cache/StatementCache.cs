using System;
using System.Data;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace ODPSample
{
  /// <summary>
  /// StatementCache: Demonstrates performance improvement when 
  ///                 Statement Caching is enabled.
  /// </summary>

  class StatementCache
  {


    static void Main(string[] args)
    {

      // Connection String with Statement Caching not enabled
      string constr = "User Id=scott;Password=<PASSWORD>;Data Source=oracle;Statement Cache Size=0;Self Tuning=false";

      Console.WriteLine("Executing 5 Statements 1000 times each, please wait... ");
      
      // Execute some statements with caching disabled and retrieve the time taken
      double totalSecCachingOff = StatementCache.MeasurePerformance(constr);
      Console.WriteLine("Statement Caching Disabled, Total Seconds: "+ totalSecCachingOff);
    

      // Connection String with Statement Caching enabled, cache size is 5
      // Statement Caching gives significant performance improvement when
      // statements get executed repeatedely
      // Hence cache size should be chosen depending on the number of statements
      // that the application repeatedly executes
      // Cache size should not exceed the size of OPEN_CURSORS as defined in init.ora
      constr = "User Id=scott;Password=<PASSWORD>;Data Source=oracle;Statement Cache Size=5;Self Tuning=false";
      
      // Execute some statements with caching enabled and retrieve the time taken
      double totalSecCachingOn = StatementCache.MeasurePerformance(constr);
      Console.WriteLine("Statement Caching Enabled, Total Seconds: "+ totalSecCachingOn);

      Console.WriteLine("Percentage gain : " + 
        ((totalSecCachingOff - totalSecCachingOn)/totalSecCachingOn)*100);

    }

    // Execute a number of Statements in a loop and gather performance numbers
    public static double MeasurePerformance(string constr)
    {

      // connect
      OracleConnection con = Connect(constr);

      // Create OracleCommand
      OracleCommand cmd = new OracleCommand();
      cmd.Connection = con;
      cmd.CommandType = CommandType.Text;

      DateTime DtStart = DateTime.Now;

      // In a loop execute five different statements
      for (int i=0; i < 5000; i++)
      {
        switch (i % 5)
        {
          case 0:
            cmd.CommandText = "select * from emp";
            break;

          case 1:
            cmd.CommandText = "select * from dept";
            break;

          case 2:
            cmd.CommandText = "select ename from emp";
            break;

          case 3:
            cmd.CommandText = "select * from dual";
            break;

          case 4:
            cmd.CommandText = "select * from all_users";
            break;

          default:
            Console.WriteLine("Should not come here");
            break;
        }

        try
        {
          OracleDataReader reader = cmd.ExecuteReader();
          reader.Dispose();
        }
        catch(Exception e)
        {
          Console.WriteLine("Error: {0}", e.Message);
        }

      }

      DateTime DtEnd = DateTime.Now;
      TimeSpan ts = DtEnd - DtStart;
      double totalSeconds = ts.TotalSeconds;

      // Close and Dispose objects
      cmd.Dispose();
      con.Close();
      con.Dispose();

      return totalSeconds;
    }

    /// <summary>
    /// Wrapper for Opening a new Connection
    /// </summary>
    /// <param name="connectStr"></param>
    /// <returns></returns>
    public static OracleConnection Connect(string connectStr)
    {
      OracleConnection con = new OracleConnection(connectStr);
      try
      {
        con.Open();
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: {0}", e.Message);
      }
      return con;
    }
  }
}

/* Copyright (c) 2017, Oracle and/or its affiliates. All rights reserved. */

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
