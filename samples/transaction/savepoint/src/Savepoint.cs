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

/**************************************************************************
@author                        :  Jagriti
@version                       :  1.0
Development Environment        :  Microsoft Visual Studio .Net
Name of the File               :  Savepoint.cs
Creation/Modification History  :
        Jagriti            22-Oct-2002        Created

Sample Overview:
This sample aims at demonstrating how Nested Transactions can be done using 
Savepoints in ODP.NET. Savepoints can be used to identify the point in a 
transaction to which the user can later rollback.
In this sample, firstly a set of Savepoints viz. a, b and c with
Data Manipulation Language(DML) operations is created. 
Then the user is given option to select a Savepoint to which he wishes to 
rollback. Hence demostrating how transactions can be rolled back to 
intermediate points by maintaining Savepoints. If more Savepoints exists, 
then user can further rollback or commit or rollback the transaction 
completely. After commit or rollback, the transaction enters a
completed state. For more information on Savepoints refer Readme.html file 
available with this sample.
NOTE: The insert, update, delete operations made to a database table data 
      are referred to DML(Data Manipulation Language) operations.
**************************************************************************/

// Include standard namespaces used in this sample
using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Savepoint
{
	public class SavepointSample
	{
		// For Database connection
		OracleConnection conn;

		// Transaction object
		OracleTransaction myTransaction;

		// Savepoint name
		string saveptName;

		/**************************************************************
		* This method is the entry point to this sample application.
		* Following tasks are done in this method:
		* 1. A connection to database is established.
		* 2. Deletes the existing data from products table, if any.
		* 3. Starts transaction
		* 4. Creates multiple Savepoints within DML Operations.
		* 5. Provides user the option to rollback the transaction to a
		*    particular Savepoint. And then transaction is rolled back
		*    upto the selected savepoint.
		***************************************************************/
		public static void Main() 
		{   
		  // Instantiating this class
		  SavepointSample savepointsample = new SavepointSample();

		  // Get database connection
		  if (savepointsample.getDBConnection())
		  {
		    // Deletes existing Products records, if any	
		    savepointsample.deleteExistingProducts();
                
		    // Create Savepoints and perform DML operations 
		    if (savepointsample.createSavepoints())
			  {
			    // Provides user the option to rollback the transaction to a
			    // particular Savepoint. Transaction is rolled back
			    // upto the selected savepoint.
			    savepointsample.chooseSavepoint();
			  }
			}
		}

		/***********************************************************
		 * The purpose of this method is to perform DML operations 
		 * on database table and create intermediate Savepoints.
		 **********************************************************/
		private Boolean createSavepoints()
		{
     	  try
		  	{
             // Calling BeginTransaction on OracleConnection object 
		     // creates an OracleTransaction object
			 myTransaction = conn.BeginTransaction();
		     Console.WriteLine("Transaction Started");
            
		     OracleCommand cmd = new OracleCommand();
			 cmd.Connection = conn;

			 // DML operation # 1: without any Savepoint 
			 string cmdText1 = " INSERT INTO Products (Product_id, Product_name)" +
				               " VALUES (1,'Product 1')";
		     cmd.CommandText = cmdText1;
			 cmd.ExecuteNonQuery();
            
			 Console.WriteLine("Inserted data for Product 1");
			 Console.WriteLine("");
              
  
			 // Create Savepoint 'a'
			 myTransaction.Save("a");
			 Console.WriteLine("Created Savepoint a");

			 // DML operation # 2: In Savepoint 'a'
			 string cmdText2 = " INSERT INTO Products (Product_id, Product_name) " +
				               " VALUES (2,'Product 2 ')";
			 cmd.CommandText = cmdText2;
	         cmd.ExecuteNonQuery();

			 Console.WriteLine("Inserted data for Product 2");
			 Console.WriteLine("");             
              
  
			 // Create Savepoint 'b'
			 myTransaction.Save("b");
			 Console.WriteLine("Created Savepoint b");
                
			 // DML operation # 3: In Savepoint 'b'
			 string cmdText3 = " INSERT INTO Products (Product_id, Product_name) " +
				               " VALUES (3,'Product 3')";
			 cmd.CommandText = cmdText3;
			 cmd.ExecuteNonQuery();
     
			 Console.WriteLine("Inserted data for Product 3");
			 Console.WriteLine("");             


			 // Create Savepoint 'c'
			 myTransaction.Save("c");
			 Console.WriteLine("Created Savepoint c");

			 // DML operation # 4: In Savepoint 'c'
			 string cmdText4 = " INSERT INTO Products (Product_id, Product_name) " +
				               " VALUES (4,'Product 4')";
			 cmd.CommandText = cmdText4;
			 cmd.ExecuteNonQuery();
			 Console.WriteLine("Inserted data for Product 4");
			 Console.WriteLine("");     
       
			 // Release all resources held by OracleCommand Object
			 cmd.Dispose();
			 	     
			 return true;
			}
			catch (Exception ex)
			{ 
			  Console.WriteLine("Execution Failed" + ex);
			  conn.Close();
			  conn.Dispose();
			  return false;
		     }
		}

		/*********************************************************************
		 * The purpose of this method is to provide user to choose option of 
		 * selecting the Savepoint to which he wishes to rollback.
		 */
		private void chooseSavepoint()
		{
		  try
		  {
		    // Display message to user to type a Savepoint name upto which he wishes to rollback
			Console.WriteLine("");  
			Console.WriteLine("Type the Savepoint name (a or b or c) upto which you wish to rollback the transaction :");
			Console.WriteLine("");  
                
			// Save user's input to a temporary variable
			saveptName = Console.ReadLine().ToLower();
				
			// Accept user's input if given as 'a', 'b', 'c'. Else display error 
			// message
			if (saveptName ==  "a" || saveptName == "b" || saveptName == "c")
			{
			  // Call 'rollbackSavepoint' method to rollback 
			  // to a particular Savepoint.
			  // Note: Savepoint name name is case insensitive
			  rollbackSavepoint(saveptName.ToLower()) ;
			}
			else
			{
			  // If Savepoint does not exists
		      Console.WriteLine("Invalid Savepoint name");
			  chooseSavepoint();					
			}
			}
			catch (Exception ex)
			{
			  Console.WriteLine("Execution Failed: "+ ex);
			  conn.Close();
			  conn.Dispose();
			}
		}
 

		/********************************************************************************
		 * Following is the purpose of this method
		 * 1. Perform rollback to a particular Savepoint.
		 * 2. After rollback, provide option for further commit or rollback for completing
		 *    the transaction.
         ********************************************************************************/
		private void rollbackSavepoint(string saveptName)
		{
			try
			{
				// Rollback to the Savepoint, given by user
				myTransaction.Rollback(saveptName);
				
				Console.WriteLine("The transaction has been rolled back upto Savepoint '" + saveptName + "'");
				Console.WriteLine("-----------------------------------------------------------------");    


				// Prompt user if he wishes to further commit or rollback
				if (saveptName == "c")
				{
					Console.WriteLine("Savepoints 'a', 'b' exists, type Savepoint name (a or b) if you wish to further rollback !");
				}
				else if (saveptName == "b")
				{
					Console.WriteLine("Savepoint 'a' exists, type Savepoint name (a) if you wish to further rollback !");
				}
            
				Console.WriteLine("Type 'commit' if you wish to commit changes");
				Console.WriteLine("Else all changes will be rolled back");
               
				String input = Console.ReadLine().ToLower();
                    
				// If user wishes to further rollback to a particular Savepoint or
				// wishes to commit data to Products table,
				// or wishes to rollback the transaction completely
				if ((input == "b" && saveptName != "b") || (input == "a" && saveptName != "a"))
				{
					saveptName = input;
					rollbackSavepoint(saveptName.ToLower());
				}
				else if (input.ToLower() == "commit" ) 
				{
					myTransaction.Commit();
					Console.WriteLine("Transaction has been committed !");
				}
				else 
				{
					myTransaction.Rollback();
					Console.WriteLine("Transaction has been rolled back !");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Execution Failed" + ex);
			}
			finally 
			{
				// Release resources held by connection object 
				conn.Close();
				conn.Dispose();
			}
		}
       
		/**************************************************************************
		 * The purpose of this method is to delete any existing records from 
		 * Products table.
		 *************************************************************************/ 
		void deleteExistingProducts()
		{
			try
			{
			  // Perform initial cleanup for Products table.  
			  Console.WriteLine("Deleting existing records from Products table...");
			  OracleCommand cmd = new OracleCommand(" DELETE FROM products", conn);
			  cmd.ExecuteNonQuery();
			  Console.WriteLine("Initial cleanup done!");
			  Console.WriteLine("");
			  cmd.Dispose();
			} 
			catch (Exception ex)
			{
			  Console.WriteLine("Execution failed : " + ex);
              conn.Close();
			  conn.Dispose();
			}
		}

		/***************************************************************************
		* The purpose of this method is to get the database connection using the
		* parameters given in ConnectionParams.cs class
		* Note: Replace the datasource parameter with your datasource value.
		***************************************************************************/
		private Boolean getDBConnection()
		{
		  try
		  {
		    // Connection Information	
			string ConnectionString = 

			// Username
			"User Id=" + ConnectionParams.Username +

			// Password
			";Password=" +ConnectionParams.Password +

			// Datasource (TNSName)
			";Data Source=" + ConnectionParams.Datasource;
					
    		// Connection to datasource, using connection parameters given above
			conn = new OracleConnection(ConnectionString);

			// Open database connection
			conn.Open();
               
			Console.WriteLine("Connection to database made successfully.");
			return true;
			}
			catch (Exception ex) // catch exception when error in connecting to database occurs
			{
			  Console.WriteLine("Connection Failed: " + ex);
			  return false;
			}
		}
	}
}
