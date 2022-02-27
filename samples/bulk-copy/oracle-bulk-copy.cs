using Oracle.ManagedDataAccess.Client;
using System;

class Program
{
    static void Main()
    {
        string connectionString = GetConnectionString();
        // Open a connection to the DB.
        using (OracleConnection sourceConnection = new OracleConnection(connectionString))
        {
            sourceConnection.Open();

            // Perform an initial count on the source/destination table.
            OracleCommand commandRowCount = new OracleCommand("SELECT COUNT(*) FROM BLOGS", sourceConnection);
            long countStart = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
            Console.WriteLine("Starting row count = {0}", countStart);

            // Get data from the source table as a OracleDataReader.
            OracleCommand commandSourceData = new OracleCommand("SELECT ID, URL FROM BLOGS", sourceConnection);
            OracleDataReader reader =  commandSourceData.ExecuteReader();

            // Open the destination connection. In the real world you would
            // not use OracleBulkCopy to move data from one table to the other
            // in the same database. This is for demonstration purposes only.
            using (OracleConnection destinationConnection = new OracleConnection(connectionString))
            {
                destinationConnection.Open();

                // Set up the bulk copy object.
                // Note that the column positions in the source data reader match the column positions in
                // the destination table so there is no need to map columns.
                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(destinationConnection))
                {
                    bulkCopy.DestinationTableName = "BLOGS";

                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(reader);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        // Close the OracleDataReader. The OracleBulkCopy object is automatically 
                        // closed at the end of the using block.
                        reader.Close();
                    }
                }

                // Perform a final count on the destination table to see how many rows were added.
                long countEnd = System.Convert.ToInt32(
                    commandRowCount.ExecuteScalar());
                Console.WriteLine("Ending row count = {0}", countEnd);
                Console.WriteLine("{0} rows were added.", countEnd - countStart);
                Console.WriteLine("Press Enter to finish.");
                Console.ReadLine();
            }
        }
    }

    private static string GetConnectionString()
    // Enter credentials below.
    {
        return "Data Source=<Data Source>; " +
            "User Id=<User Id>;" +
            "Password=<Password>;";
    }
}

/* Copyright (c) 2022 Oracle and/or its affiliates. All rights reserved. */
/* Copyright (c) Microsoft                                               */

/******************************************************************************
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *   limitations under the License.
 * 
 *****************************************************************************/
