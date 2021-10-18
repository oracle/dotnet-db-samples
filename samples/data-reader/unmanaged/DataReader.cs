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
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

class VisibleFieldCountSample
{
  static void Main(string[] args)
  {
    string constr = "User Id=scott; Password=<PASSWORD>; Data Source=oracle;";
    DbProviderFactory factory =
            DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Client");

    using (DbConnection conn = factory.CreateConnection())
    {
      conn.ConnectionString = constr;
      try
      {
        conn.Open();
        OracleCommand cmd = (OracleCommand)factory.CreateCommand();
        cmd.Connection = (OracleConnection)conn;

        //to gain access to ROWIDs of the table
        cmd.AddRowid = true;
        cmd.CommandText = "select empno, ename from emp;";

        OracleDataReader reader = cmd.ExecuteReader();
        
        int visFC = reader.VisibleFieldCount; //Results in 2
        int hidFC = reader.HiddenFieldCount;  // Results in 1

        Console.Write("Visible field count: " + visFC);
        Console.Write("Hidden field count: " + hidFC);

        reader.Dispose();
        cmd.Dispose();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
      }
    }
  }
}

 
