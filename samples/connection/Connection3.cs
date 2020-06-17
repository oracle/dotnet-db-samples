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
    string constr = "User Id=scott; Password=<PASSWORD>; Data Source=oracle;";
    string ProviderName = "Oracle.ManagedDataAccess.Client";

    DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);

    using (DbConnection conn = factory.CreateConnection())
    {
      try
      {
        conn.ConnectionString = constr;
        conn.Open();

        //Get Restrictions
        DataTable dtRestrictions =
          conn.GetSchema(DbMetaDataCollectionNames.Restrictions);
        
        DataView dv = dtRestrictions.DefaultView;

        dv.RowFilter = "CollectionName = 'Columns'";
        dv.Sort = "RestrictionNumber";

        for (int i = 0; i < dv.Count; i++)
          Console.WriteLine("{0} (default) {1}" , 
                            dtRestrictions.Rows[i]["RestrictionName"], 
                            dtRestrictions.Rows[i]["RestrictionDefault"]);

        //Set restriction string array
        string[] restrictions = new string[3];

        //Get all columns from all tables owned by "SCOTT"
        restrictions[0] = "SCOTT";
        DataTable dtAllScottCols = conn.GetSchema("Columns", restrictions);

        // clear collection
        for (int i = 0; i < 3; i++)
          restrictions[i] = null;

        //Get all columns from all tables named "EMP" owned by any 
        //owner/schema
        restrictions[1] = "EMP";
        DataTable dtAllEmpCols = conn.GetSchema("Columns", restrictions);

        // clear collection
        for (int i = 0; i < 3; i++)
          restrictions[i] = null;

        //Get columns named "EMPNO" from tables named "EMP", 
        //owned by any owner/schema
        restrictions[1] = "EMP";
        restrictions[2] = "EMPNO";
        DataTable dtAllScottEmpCols = conn.GetSchema("Columns", restrictions);

        // clear collection
        for (int i = 0; i < 3; i++)
          restrictions[i] = null;

        //Get columns named "EMPNO" from all
        //tables, owned by any owner/schema
        restrictions[2] = "EMPNO";
        DataTable dtAllEmpNoCols = conn.GetSchema("Columns", restrictions);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.Source);
      }
    }
  }
}
