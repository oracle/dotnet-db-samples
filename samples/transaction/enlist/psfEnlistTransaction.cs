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
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Transactions;

class psfEnlistTransaction
{
  static void Main()
  {
    int retVal = 0;
    string providerName = "Oracle.ManagedDataAccess.Client";
    string constr =
           @"User Id=scott;Password=<PASSWORD>;Data Source=oracle;enlist=dynamic";

    // Get the provider factory.
    DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

    try
    {
      // Create a committable transaction object.
      CommittableTransaction cmtTx = new CommittableTransaction();

      // Open a connection to the DB.
      DbConnection conn1 = factory.CreateConnection();
      conn1.ConnectionString = constr;
      conn1.Open();

      // enlist the connection with the commitable transaction.
      conn1.EnlistTransaction(cmtTx);

      // Create a command to execute the sql statement.
      DbCommand cmd1 = factory.CreateCommand();
      cmd1.Connection = conn1;
      cmd1.CommandText = @"insert into dept (deptno, dname, loc) 
                                        values (99, 'st', 'lex')";

      // Execute the SQL statement to insert one row in DB.
      retVal = cmd1.ExecuteNonQuery();
      Console.WriteLine("Rows to be affected by cmd1: {0}", retVal);

      // commit/rollback the transaction.
      cmtTx.Commit();   // commits the txn.
      //cmtTx.Rollback(); // rolls back the txn.

      // close and dispose the connection
      conn1.Close();
      conn1.Dispose();
      cmd1.Dispose();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      Console.WriteLine(ex.StackTrace);
    }
  }
}
