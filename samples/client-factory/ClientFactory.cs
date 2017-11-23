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

class FactorySample
{
  static void Main()
  {
    string constr = "user id=scott;password=tiger;data source=oracle";

    DbProviderFactory factory =
            DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Client");

    DbConnection conn = factory.CreateConnection();

    try
    {
      conn.ConnectionString = constr;
      conn.Open();

      DbCommand cmd = factory.CreateCommand();
      cmd.Connection = conn;
      cmd.CommandText = "select * from emp";

      DbDataReader reader = cmd.ExecuteReader();
      while (reader.Read())
        Console.WriteLine(reader["EMPNO"] + " : " + reader["ENAME"]);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      Console.WriteLine(ex.StackTrace);
    }
  }
}

