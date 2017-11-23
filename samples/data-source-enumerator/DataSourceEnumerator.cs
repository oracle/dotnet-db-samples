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

class DataSourceEnumSample
{
  static void Main()
  {
    string ProviderName = "Oracle.ManagedDataAccess.Client";

    DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);

    if (factory.CanCreateDataSourceEnumerator)
    {
      DbDataSourceEnumerator dsenum = factory.CreateDataSourceEnumerator();
      DataTable dt = dsenum.GetDataSources();

      // Print the first column/row entry in the DataTable
      Console.WriteLine(dt.Columns[0] + " : " + dt.Rows[0][0]);
      Console.WriteLine(dt.Columns[1] + " : " + dt.Rows[0][1]);
      Console.WriteLine(dt.Columns[2] + " : " + dt.Rows[0][2]);
      Console.WriteLine(dt.Columns[3] + " : " + dt.Rows[0][3]);
      Console.WriteLine(dt.Columns[4] + " : " + dt.Rows[0][4]);
    }
    else
      Console.Write("Data source enumeration is not supported by provider");
  }
}

