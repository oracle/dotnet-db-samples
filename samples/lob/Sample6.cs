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

using System;
using System.Data;
using System.Text;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Sample6
{
  /// <summary>
  /// Sample6: Demonstrates LOB updates using row-level locking.
  /// </summary>
  class Sample6
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
      // Connect
      string constr = "User Id=scott;Password=tiger;Data Source=oracle";
      OracleConnection con = Connect(constr);
           
      // Setup
      Setup(con);
      
      OracleTransaction txn = con.BeginTransaction();
      OracleCommand     cmd = new OracleCommand("",con);

      try 
      {
        // Select the LOB with the primary key
        // The primary key will be used for row-level locking
        cmd.CommandText = "select STORY, THEKEY from multimedia_tab where THEKEY = 1";
 
        OracleDataReader reader = cmd.ExecuteReader();
        reader.Read();
        OracleClob clob = reader.GetOracleClobForUpdate(0);  // Lock the row
        Console.WriteLine("Old Data: {0}", clob.Value);
      
        // Modify the CLOB column of the row
        string ending = " The end.";
        clob.Append(ending.ToCharArray(), 0, ending.Length);      

        // Release the lock
        txn.Commit();

        // Fetch the new data; transaction or locking not required.
        cmd.CommandText = "select STORY from multimedia_tab where THEKEY = 1";
        reader = cmd.ExecuteReader();
        reader.Read();
        clob = reader.GetOracleClob(0);
        Console.WriteLine("New Data: {0}", clob.Value);
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: {0}", e.Message);
      }
      finally
      {
        // Dispose OracleCommand object
        cmd.Dispose();

        // Close and Dispose OracleConnection object
        con.Close();
        con.Dispose();
      }
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

    /// <summary>
    /// Setup the necessary Tables & Test Data
    /// </summary>
    /// <param name="connectStr"></param>
    public static void Setup(OracleConnection con)
    {
      StringBuilder blr;
      OracleCommand cmd = new OracleCommand("", con);

      blr = new StringBuilder();
      blr.Append("DROP TABLE multimedia_tab");
      cmd.CommandText = blr.ToString();
      try 
      {
        cmd.ExecuteNonQuery();
      }
      catch 
      { 
      }
      
      blr = new StringBuilder();
      blr.Append("CREATE TABLE multimedia_tab(thekey NUMBER(4) PRIMARY KEY,");
      blr.Append("story CLOB, sound BLOB)");
      cmd.CommandText = blr.ToString();
      try 
      {
        cmd.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: {0}", e.Message);
      }

      blr = new StringBuilder();
      blr.Append("INSERT INTO multimedia_tab values(");
      blr.Append("1,");
      blr.Append("'This is a long story. Once upon a time ...',");
      blr.Append("'656667686970717273747576777879808182838485')");
      cmd.CommandText = blr.ToString();
      try 
      {
        cmd.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: {0}", e.Message);
      }
    }
  }
}
