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

        //Get MetaDataCollections and write to an XML file.
        //This is equivalent to GetSchema()
        DataTable dtMetadata =
          conn.GetSchema(DbMetaDataCollectionNames.MetaDataCollections);
        dtMetadata.WriteXml(ProviderName + "_MetaDataCollections.xml");

        //Get Restrictions and write to an XML file.
        DataTable dtRestrictions =
          conn.GetSchema(DbMetaDataCollectionNames.Restrictions);
        dtRestrictions.WriteXml(ProviderName + "_Restrictions.xml");

        //Get DataSourceInformation and write to an XML file.
        DataTable dtDataSrcInfo =
          conn.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);
        dtDataSrcInfo.WriteXml(ProviderName + "_DataSourceInformation.xml");

        //DataTypes and write to an XML file.
        DataTable dtDataTypes =
          conn.GetSchema(DbMetaDataCollectionNames.DataTypes);
        dtDataTypes.WriteXml(ProviderName + "_DataTypes.xml");

        //Get ReservedWords and write to an XML file.
        DataTable dtReservedWords =
          conn.GetSchema(DbMetaDataCollectionNames.ReservedWords);
        dtReservedWords.WriteXml(ProviderName + "_ReservedWords.xml");

        //Get all the tables and write to an XML file.
        DataTable dtTables = conn.GetSchema("Tables");
        dtTables.WriteXml(ProviderName + "_Tables.xml");

        //Get all the views and write to an XML file.
        DataTable dtViews = conn.GetSchema("Views");
        dtViews.WriteXml(ProviderName + "_Views.xml");

        //Get all the columns and write to an XML file.
        DataTable dtColumns = conn.GetSchema("Columns");
        dtColumns.WriteXml(ProviderName + "_Columns.xml");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
      }
    }
  }
}


