using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ODPArrayBind
{
    /// <summary>
    /// Sample: Demonstrates ODP.NET array binding
    /// </summary>
    class ArrayBind
    {
        static void Main(string[] args)
        {
            // Connect
            // This sample code's DEPT table shares the same characteristics as the SCOTT schema's DEPT table.
            string connectStr = "User Id=<USER ID>;Password=<PASSWORD>;Data Source=<TNS ALIAS>";

            // Clear rows from past sample code executions
            Setup(connectStr);

            // Initialize array of data
            int[] myArrayDeptNo = new int[4] { 1, 2, 3, 4 };
            String[] myArrayDeptName = { "Dev", "QA", "PM", "Integration" };
            String[] myArrayDeptLoc = { "California", "Arizona", "Texas", "Oregon" };

            OracleConnection connection = new OracleConnection(connectStr);
            OracleCommand command = new OracleCommand(
              "insert into dept values (:deptno, :deptname, :loc)", connection);

            // Set the array size to 4. This applies to all the command's associated parameters.
            command.ArrayBindCount = 4;

            // Deptno parameter
            OracleParameter deptNoParam = new OracleParameter("deptno", OracleDbType.Int32);
            deptNoParam.Direction = ParameterDirection.Input;
            deptNoParam.Value = myArrayDeptNo;
            command.Parameters.Add(deptNoParam);

            // Deptname parameter
            OracleParameter deptNameParam = new OracleParameter("deptname", OracleDbType.Varchar2);
            deptNameParam.Direction = ParameterDirection.Input;
            deptNameParam.Value = myArrayDeptName;
            command.Parameters.Add(deptNameParam);

            // Loc parameter
            OracleParameter deptLocParam = new OracleParameter("loc", OracleDbType.Varchar2);
            deptLocParam.Direction = ParameterDirection.Input;
            deptLocParam.Value = myArrayDeptLoc;
            command.Parameters.Add(deptLocParam);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("{0} rows inserted", command.ArrayBindCount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Execution failed:" + e.Message);
            }
            finally
            {
                // Dispose connection and command used server side resource
                connection.Close();
                command.Dispose();
                connection.Dispose();
            }
        }

        public static void Setup(string connectStr)
        {
            int[] myArrayDeptNo = new int[4] { 1, 2, 3, 4 };

            OracleConnection conn = new OracleConnection(connectStr);
            OracleCommand cmd = new OracleCommand("delete dept where deptno = :1", conn);

            // Bind with a 4 item array
            cmd.ArrayBindCount = 4;

            OracleParameter param1 = new OracleParameter();
            param1.OracleDbType = OracleDbType.Int32;
            param1.Value = myArrayDeptNo;

            cmd.Parameters.Add(param1);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Setup Failed:{0}", e.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
            }
        }
    }
}

/* Copyright (c) 2017, 2024 Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/
