using System;
using System.Data;
using System.Text;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace ODPSample
{
  class AssocArray
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Demo: PL/SQL Associative Array");

      // Provide password and connect
      string connectStr = "User Id=scott;Password=<PASSWORD>;Data Source=oracle";

      // Setup the Tables for sample
      Setup(connectStr);

      OracleConnection connection = new OracleConnection(connectStr);
      OracleCommand cmd = new OracleCommand("begin MyPack.TestVarchar2(:1, :2, :3);end;", 
        connection);

      OracleParameter param1 = cmd.Parameters.Add("param1", OracleDbType.Varchar2);
      OracleParameter param2 = cmd.Parameters.Add("param2", OracleDbType.Varchar2);
      OracleParameter param3 = cmd.Parameters.Add("param3", OracleDbType.Varchar2);

      // Setup the direction
      param1.Direction = ParameterDirection.Input;
      param2.Direction = ParameterDirection.InputOutput;
      param3.Direction = ParameterDirection.Output;

      // Specify that we are binding PL/SQL Associative Array
      param1.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
      param2.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
      param3.CollectionType = OracleCollectionType.PLSQLAssociativeArray;

      param1.Value = new string[3]{"Input1",
                                   "Input2",
                                   "Input3"};
      param2.Value = new string[3]{"Inout1",
                                   "Inout2",
                                   "Inout3"};
      param3.Value = null;

      // Specify the maximum number of elements in the PL/SQL Associative
      // Array
      param1.Size = 3;
      param2.Size = 3;
      param3.Size = 3;

      // Setup the ArrayBind Size for param1
      param1.ArrayBindSize = new int[3]{13,14,13};  

      // Setup the ArrayBind Status for param1
      param1.ArrayBindStatus = new OracleParameterStatus[3]{
        OracleParameterStatus.Success,
        OracleParameterStatus.Success,
        OracleParameterStatus.Success};
      
      // Setup the ArrayBind Size for param2
      param2.ArrayBindSize = new int[3]{20,20,20};
      
      // Setup the ArrayBind Size for param3
      param3.ArrayBindSize = new int[3]{20,20,20};

      try 
      {
        connection.Open();
        cmd.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      Console.WriteLine("Results:");
      Display(cmd.Parameters);
    }

    public static void Setup(string connectStr)
    {
      OracleConnection connection = new OracleConnection(connectStr);
      OracleCommand    command    = new OracleCommand("", connection);
      
      try 
      {
        connection.Open();

        command.CommandText = "drop table T1";
        try 
        {
          command.ExecuteNonQuery();
        }
        catch
        {
        }

        command.CommandText = "CREATE TABLE T1(COL1 number, COL2 varchar2(20))";
        command.ExecuteNonQuery();

        StringBuilder hdr = new StringBuilder();
        hdr.Append("CREATE or replace PACKAGE MYPACK AS ");
        hdr.Append("TYPE AssocArrayVarchar2_t is table of VARCHAR(20) index by BINARY_INTEGER;");
        hdr.Append("  PROCEDURE TestVarchar2(");
        hdr.Append("    Param1 IN     AssocArrayVarchar2_t,");
        hdr.Append("    Param2 IN OUT AssocArrayVarchar2_t,");
        hdr.Append("    Param3    OUT AssocArrayVarchar2_t);");
        hdr.Append("  END MYPACK;");
        command.CommandText = hdr.ToString();
        command.ExecuteNonQuery();

        StringBuilder body = new StringBuilder();
        body.Append("CREATE or REPLACE package body MYPACK as ");
        body.Append("  PROCEDURE TestVarchar2(");
        body.Append("  Param1 IN     AssocArrayVarchar2_t,");
        body.Append("  Param2 IN OUT AssocArrayVarchar2_t,");
        body.Append("  Param3    OUT AssocArrayVarchar2_t)");
        body.Append("  IS");
        body.Append("  i integer;");
        body.Append("  BEGIN");
        body.Append("    -- copy a few elements from Param2 to Param1\n");
        body.Append("    Param3(1) := Param2(1);");
        body.Append("    Param3(2) := NULL;");
        body.Append("    Param3(3) := Param2(3);");
        body.Append("    -- copy all elements from Param1 to Param2\n");
        body.Append("    Param2(1) := Param1(1);");
        body.Append("    Param2(2) := Param1(2);");
        body.Append("    Param2(3) := Param1(3);");
        body.Append("    -- insert some values to db\n");
        body.Append("    FOR i IN 1..3 LOOP");
        body.Append("      insert into T1 values(i,Param2(i));");
        body.Append("    END LOOP;");
        body.Append("  END TestVarchar2;");
        body.Append("END MYPACK;");


        command.CommandText = body.ToString();
        command.ExecuteNonQuery();

        command.CommandText="Commit";
        command.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        Console.WriteLine(command.CommandText + ":" + e.Message);
      }
      
    }
      
    public static void Display(OracleParameterCollection collection)
    {
      foreach(OracleParameter p in collection)
      {
        Console.Write(p.ParameterName + ": ");
        for(int i=0;i<3;i++)
        {
          if (p.Value is OracleString[])
            Console.Write((p.Value as OracleString[])[i]);
          else 
            Console.Write((p.Value as string[])[i]);
          Console.Write(" ");
        }
        Console.WriteLine();
      }
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
