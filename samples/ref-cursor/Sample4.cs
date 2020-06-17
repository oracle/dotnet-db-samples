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

namespace Sample4
{
  /// <summary>
  /// Sample 4: Demonstrates how a DataSet can be populated from a 
  ///           REF Cursor. The sample also demonstrates how a REF 
  ///           Cursor can be updated.
  /// </summary>
  class Sample4
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
      OracleCommand cmd = new OracleCommand("TEST.Ret1Cur", con);
      cmd.CommandType = CommandType.StoredProcedure;
      
      // Bind
      // TEST.Ret1Cur is a function so ParameterDirection is ReturnValue.
      OracleParameter param = cmd.Parameters.Add("refcursor",
                                                  OracleDbType.RefCursor);
      param.Direction = ParameterDirection.ReturnValue;

      // Create an OracleDataAdapter
      OracleDataAdapter da = new OracleDataAdapter(cmd);

      try 
      {
        // 1. Demostrate populating a DataSet with RefCursor
        // Populate a DataSet
        DataSet ds = new DataSet();
        da.FillSchema(ds, SchemaType.Source, "myRefCursor");
        da.Fill(ds, "myRefCursor");
      
        // Obtain the row which we want to modify
        DataRow[] rowsWanted = ds.Tables["myRefCursor"].Select("THEKEY = 1");

        // 2. Demostrate how to update with RefCursor
        // Update the "story" column
        rowsWanted[0]["story"] = "New story";

        // Setup the update command on the DataAdapter
        OracleCommand updcmd = new OracleCommand("TEST.UpdateREFCur", con);
        updcmd.CommandType = CommandType.StoredProcedure;
      
        OracleParameter param1 = updcmd.Parameters.Add("myStory",
          OracleDbType.Varchar2, 32);
        param1.SourceVersion = DataRowVersion.Current;
        param1.SourceColumn = "STORY";
        OracleParameter param2 = updcmd.Parameters.Add("myClipId", 
          OracleDbType.Decimal);
        param2.SourceColumn = "THEKEY";      
        param2.SourceVersion = DataRowVersion.Original;

        da.UpdateCommand = updcmd;

        // Update
        da.Update(ds, "myRefCursor");
        Console.WriteLine("Data has been updated.");
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
