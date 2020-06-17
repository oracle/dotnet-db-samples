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

class psfTxnScope
{
  static void Main()
  {
    int retVal = 0;
    string providerName = "Oracle.ManagedDataAccess.Client";
    string constr =
           @"User Id=scott;Password=<PASSWORD>;Data Source=oracle;";

    // Get the provider factory.
    DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

    try
    {
      // Create a TransactionScope object, (It will start an ambient
      // transaction automatically).
      using (TransactionScope scope = new TransactionScope())
      {
        // Create first connection object.
        using (DbConnection conn1 = factory.CreateConnection())
        {
          // Set connection string and open the connection. this connection 
          // will be automatically enlisted in a distributed transaction.
          conn1.ConnectionString = constr;
          conn1.Open();

          // Create a command to execute the sql statement.
          DbCommand  cmd1 = factory.CreateCommand();
          cmd1.Connection = conn1;
          cmd1.CommandText = @"insert into dept (deptno, dname, loc) 
                                        values (99, 'st', 'lex')";

          // Execute the SQL statement to insert one row in DB.
          retVal = cmd1.ExecuteNonQuery();
          Console.WriteLine("Rows to be affected by cmd1: {0}", retVal);

          // Close the connection and dispose the command object.
          conn1.Close();
          conn1.Dispose();
          cmd1.Dispose();
        }

        // The Complete method commits the transaction. If an exception has
        // been thrown or Complete is not called then the transaction is 
        // rolled back.
        scope.Complete();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      Console.WriteLine(ex.StackTrace);
    }
  }
}
