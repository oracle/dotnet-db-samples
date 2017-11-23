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
using Oracle.ManagedDataAccess.Client;

namespace ODPSample
{
  /// <summary>
  /// Sample: Demonstrates array binding
  /// </summary>
  class ArrayBind
  {
    static void Main(string[] args)
    {
      // Connect
      string connectStr = "User Id=scott;Password=tiger;Data Source=oracle";

      // Setup the Tables for sample
      Setup(connectStr);

      // Initialize array of data
      int[]    myArrayDeptNo   = new int[3]{1, 2, 3};
      String[] myArrayDeptName = {"Dev", "QA", "Facility"};
      String[] myArrayDeptLoc  = {"New York", "Maryland", "Texas"};
      
      OracleConnection connection = new OracleConnection(connectStr);
      OracleCommand    command    = new OracleCommand (
        "insert into dept values (:deptno, :deptname, :loc)", connection);
          
      // Set the Array Size to 3. This applied to all the parameter in 
      // associated with this command
      command.ArrayBindCount = 3;

      // deptno parameter
      OracleParameter deptNoParam = new OracleParameter("deptno",OracleDbType.Int32);
      deptNoParam.Direction       = ParameterDirection.Input;
      deptNoParam.Value           = myArrayDeptNo;
      command.Parameters.Add(deptNoParam);

      // deptname parameter
      OracleParameter deptNameParam = new OracleParameter("deptname", OracleDbType.Varchar2);
      deptNameParam.Direction       = ParameterDirection.Input;
      deptNameParam.Value           = myArrayDeptName;
      command.Parameters.Add(deptNameParam);

      // loc parameter
      OracleParameter deptLocParam = new OracleParameter("loc", OracleDbType.Varchar2);
      deptLocParam.Direction       = ParameterDirection.Input;
      deptLocParam.Value           = myArrayDeptLoc;
      command.Parameters.Add(deptLocParam);

      try 
      {
        connection.Open();
        command.ExecuteNonQuery();
        Console.WriteLine("{0} Rows Inserted", command.ArrayBindCount);
      }
      catch (Exception e)
      {
        Console.WriteLine("Execution Failed:" + e.Message);
      }
      finally
      {
        // connection, command used server side resource, dispose them
        // asap to conserve resource
        connection.Close();
        command.Dispose();
        connection.Dispose();
      }
    }

    public static void Setup(string connectStr)
    {
      int[]  myArrayDeptNo   = new int[3]{1, 2, 3};

      OracleConnection conn = new OracleConnection(connectStr);
      OracleCommand cmd     = new OracleCommand("delete dept where deptno = :1", conn);

      // Bind with an array of 3 items
      cmd.ArrayBindCount = 3;

      OracleParameter param1 = new OracleParameter();
      param1.OracleDbType  = OracleDbType.Int32;
      param1.Value = myArrayDeptNo;

      cmd.Parameters.Add(param1);

      try
      {
        conn.Open();
        cmd.ExecuteNonQuery(); 
      }
      catch (Exception e)
      {
        Console.WriteLine("Setup Failed:{0}" ,e.Message);
      }
      finally
      {
        conn.Close();
        cmd.Dispose();
      }
    }
  }
}
