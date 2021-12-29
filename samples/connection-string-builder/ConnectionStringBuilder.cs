using System;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using System.Collections;

class ConnectionStringBuilderSample
{
  static void Main(string[] args)
  {
    string connString = "user id=scott;password=<PASSWORD>;Data source=oracle;";
    bool bRet = false;
    
    // Create an instance of OracleConnectionStringBuilder
    OracleConnectionStringBuilder connStrBuilder = 
      new OracleConnectionStringBuilder(connString);
    
    // Add a new key/value to the connection string
    connStrBuilder.Add("pooling", false);
    
    // Modify the existing value
    connStrBuilder["Data source"] = "inst1";
    
    // Remove an entry from the connection string
    bRet = connStrBuilder.Remove("pooling");

    //ContainsKey indicates whether or not the specific key exist
    //returns true even if the user has not specified it explicitly
    Console.WriteLine("Enlist exist: " + 
              connStrBuilder.ContainsKey("Enlist"));

    //returns false
    connStrBuilder.ContainsKey("Invalid");

    // ShouldSerialize indicates whether or not a specific key 
    // exists in connection string inherited from DbConnectionStringBuilder.
    // returns true if the key is explicitly added the user otherwise false;
    // this will return false as this key doesn't exists.
    connStrBuilder.ShouldSerialize("user"); 

    // returns false because this key is nott added by user explicitly.
    connStrBuilder.ShouldSerialize("Enlist");

    // IsFixedSize [read-only property]
    Console.WriteLine("Connection String is fixed size only: "
                            + connStrBuilder.IsFixedSize);
    Console.WriteLine("Key/Value Pair Count: " + connStrBuilder.Count);

    //adding a new key which is not supported by the provider 
    //is not allowed.
    try
    {
      //this will throw an exception.
      connStrBuilder.Add("NewKey", "newValue");
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }

    Console.WriteLine("Key/Value Pair Count: " + connStrBuilder.Count);

    //modifying a existing key is allowed.
    connStrBuilder.Add("Enlist", false);
    Console.WriteLine("Key/Value Pair Count: " + connStrBuilder.Count);
 
    // Get all the keys and values supported by the provider.
    ICollection keyCollection = connStrBuilder.Keys;   
    int cnt = keyCollection.Count;
    string[] keyAry = new string[cnt]; 
    keyCollection.CopyTo(keyAry, 0);
    Array.Sort(keyAry);

    foreach (string key in keyAry)
    {
       Console.WriteLine("Key: {0}     Value: {1} \n"
          ,key, connStrBuilder[key]);
    }    
  }
}

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
