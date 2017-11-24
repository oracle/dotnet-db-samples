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

namespace Sample2
{
  /// <summary>
  /// Sample 2: Demonstrates how a REF Cursor is obtained as an 
  ///           OracleDataReader through the use of an OracleRefCursor object.
  /// </summary>
  class Sample2
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

      // Set the command
      OracleCommand cmd = new OracleCommand(
        "begin open :1 for select * from multimedia_tab where thekey = 1; end;",
        con);

      cmd.CommandType = CommandType.Text;
      
      // Bind 
      OracleParameter oparam = cmd.Parameters.Add("refcur", OracleDbType.RefCursor);
      oparam.Direction = ParameterDirection.Output;

      try 
      {
        // Execute command
        cmd.ExecuteNonQuery();

        // Obtain the OracleDataReader from the REF Cursor parameter
        // oparam.Value returns an OracleRefCursor object.
        // GetDataReader is a method of OracleRefCursor that returns an OracleDataReader object.
        OracleDataReader reader = (OracleDataReader)((OracleRefCursor)(oparam.Value)).GetDataReader();

        // show the first row
        reader.Read();

        // Print out SCOTT.MULTIMEDIA_TAB THEKEY column
        Console.WriteLine("THEKEY: {0}", reader.GetDecimal(0));

        // Print out SCOTT.MULTIMEDIA_TAB STORY column
        Console.WriteLine("STORY : {0}", reader.GetString(1));
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
      OracleCommand cmd = new OracleCommand("",con);

      // Create multimedia table
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
