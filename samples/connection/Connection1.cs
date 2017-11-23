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

class GetSchemaSample
{
  static void Main(string[] args)
  {
    string constr = "User Id=scott; Password=tiger; Data Source=oracle;";
    string ProviderName = "Oracle.ManagedDataAccess.Client";

    DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);

    using (DbConnection conn = factory.CreateConnection())
    {
      try
      {
        conn.ConnectionString = constr;
        conn.Open();

        //Get all the schema collections and write to an XML file. 
        //The XML file name is Oracle.ManagedDataAccess.Client_Schema.xml
        DataTable dtSchema = conn.GetSchema();
        dtSchema.WriteXml(ProviderName + "_Schema.xml");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
      }
    }
  }
}

