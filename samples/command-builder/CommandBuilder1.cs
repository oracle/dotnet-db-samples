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

class SchemaSeperatorSample
{
  static void Main(string[] args)
  {
    try
    {
      OracleCommandBuilder cmdBuilder = new OracleCommandBuilder();

      //schemaSeparator is dot(.)
      Console.WriteLine("schemaSeparator is {0}", 
                            cmdBuilder.SchemaSeparator);

      //set the schemaseparator, only '.' is allowed.
      cmdBuilder.SchemaSeparator = ".";

      // the only acceptable value for this property is a dot (.)
      // Hence the following line will throw NotSupportedException
      cmdBuilder.SchemaSeparator = "!";
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      Console.WriteLine(ex.StackTrace);
    }
  }
}

