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
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Sample3
{
  /// <summary>
  /// Sample 3: Demonstrates how an OracleClob object is obtained 
  ///           from an output parameter of a stored procedure
  /// </summary>
  class Sample3
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
      // Connect
      string constr = "User Id=scott;Password=<PASSWORD>;Data Source=oracle";
      OracleConnection con = Connect(constr);
      
      // Setup
      Setup(con);
      
      // Set the command
      OracleCommand cmd = new OracleCommand("", con);
      cmd.CommandText = "SelectStory";
      cmd.CommandType = CommandType.StoredProcedure; 

      // Bind the OraLob Object
      OracleParameter param = cmd.Parameters.Add("clobdata", 
                                                  OracleDbType.Clob);
      param.Direction = ParameterDirection.Output;

      // Execute command
      try
      {
        cmd.ExecuteNonQuery();

        // Obtain LOB data as a .NET Type.
        // cmd.Parameters[0].value is an OracleClob object.
        // OracleClob.Value property retuns CLOB data as a string.  
        string lob_data = (string) ((OracleClob)(cmd.Parameters[0].Value)).Value;

        // Print out the text
        Console.WriteLine("Data is: " + lob_data);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
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

      // Build a SQL that creates stored procedure 
      StringBuilder sql = new StringBuilder();
      sql.Append("create or replace procedure SelectStory ( ");
      sql.Append("clob_data OUT CLOB) as ");
      sql.Append("begin ");
      sql.Append(" select story into clob_data from multimedia_tab where thekey = 1; ");
      sql.Append("end SelectStory;");
      cmd.CommandText = sql.ToString();
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
