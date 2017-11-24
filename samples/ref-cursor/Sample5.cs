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

namespace Sample5
{
  /// <summary>
  /// Sample 5: Demonstrates how a DataSet can be populated from an
  ///           OracleRefCursor object.
  /// </summary>
  class Sample5
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
      OracleCommand cmd = new OracleCommand("TEST.Get1Cur", con);
      cmd.CommandType = CommandType.StoredProcedure;
      
      // Bind 
      cmd.Parameters.Add("refcursor1", OracleDbType.RefCursor);
      cmd.Parameters[0].Direction = ParameterDirection.ReturnValue;

      try 
      {
        // Execute command; Have the parameters populated
        cmd.ExecuteNonQuery();
      
        // Create the OracleDataAdapter
        OracleDataAdapter da = new OracleDataAdapter(cmd);

        // Populate a DataSet with refcursor1.
        DataSet ds = new DataSet();
        da.Fill(ds, "refcursor1", (OracleRefCursor)(cmd.Parameters["refcursor1"].Value));
      
        // Print out the field count the REF Cursor
        Console.WriteLine("Field count: " + ds.Tables["refcursor1"].Columns.Count);
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

      // Create Package Header
      blr = new StringBuilder();
      blr.Append("CREATE OR REPLACE PACKAGE TEST is ");
      blr.Append("TYPE refcursor is ref cursor;");
      blr.Append("FUNCTION Ret1Cur return refCursor;");

      blr.Append("PROCEDURE Get1CurOut(p_cursor1 out refCursor);");

      blr.Append("FUNCTION Get3Cur (p_cursor1 out refCursor,");
      blr.Append("p_cursor2 out refCursor)");
      blr.Append("return refCursor;");

      blr.Append("FUNCTION Get1Cur return refCursor;");

      blr.Append("PROCEDURE UpdateRefCur(new_story in VARCHAR,");
      blr.Append("clipid in NUMBER);");

      blr.Append("PROCEDURE GetStoryForClip1(p_cursor out refCursor);");

      blr.Append("PROCEDURE GetRefCurData (p_cursor out refCursor,myStory out VARCHAR2);");
      blr.Append("end TEST;");

      cmd.CommandText = blr.ToString();

      try
      {
        cmd.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: {0}", e.Message);
      }

      // Create Package Body
      blr = new StringBuilder();
      
      blr.Append("create or replace package body TEST is ");

      blr.Append("FUNCTION Ret1Cur return refCursor is ");
      blr.Append("p_cursor refCursor; ");
      blr.Append("BEGIN ");
      blr.Append("open p_cursor for select * from multimedia_tab; ");
      blr.Append("return (p_cursor); ");
      blr.Append("END Ret1Cur; ");

      blr.Append("PROCEDURE Get1CurOut(p_cursor1 out refCursor) is ");
      blr.Append("BEGIN ");
      blr.Append("OPEN p_cursor1 for select * from emp; ");
      blr.Append("END Get1CurOut; ");

      blr.Append("FUNCTION Get3Cur (p_cursor1 out refCursor, ");
      blr.Append("p_cursor2 out refCursor)");
      blr.Append("return refCursor is ");
      blr.Append("p_cursor refCursor; ");
      blr.Append("BEGIN ");
      blr.Append("open p_cursor for select * from multimedia_tab; ");
      blr.Append("open p_cursor1 for select * from emp; ");
      blr.Append("open p_cursor2 for select * from dept; ");
      blr.Append("return (p_cursor); ");
      blr.Append("END Get3Cur; ");

      blr.Append("FUNCTION Get1Cur return refCursor is ");
      blr.Append("p_cursor refCursor; ");
      blr.Append("BEGIN ");
      blr.Append("open p_cursor for select * from multimedia_tab; ");
      blr.Append("return (p_cursor); ");
      blr.Append("END Get1Cur; ");

      blr.Append("PROCEDURE UpdateRefCur(new_story in VARCHAR, ");
      blr.Append("clipid in NUMBER) is ");
      blr.Append("BEGIN ");
      blr.Append("Update multimedia_tab set story = new_story where thekey = clipid; ");
      blr.Append("END UpdateRefCur; ");

      blr.Append("PROCEDURE GetStoryForClip1(p_cursor out refCursor) is ");
      blr.Append("BEGIN ");
      blr.Append("open p_cursor for ");
      blr.Append("Select story from multimedia_tab where thekey = 1; ");
      blr.Append("END GetStoryForClip1; ");
      
      blr.Append("PROCEDURE GetRefCurData (p_cursor out refCursor,");
      blr.Append("myStory out VARCHAR2) is ");
      blr.Append("BEGIN ");
      blr.Append("FETCH p_cursor into myStory; ");
      blr.Append("END GetRefCurData; ");

      blr.Append("end TEST;");

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
