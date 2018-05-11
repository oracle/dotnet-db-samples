using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace BestPractices
{
    class Program
    {
        static void Main(string[] args)
        {
          

            string conString = "User Id=hr;Password=hr;Data Source=localhost:1521/orclpdb;" +
                    "Pooling=true;Min Pool Size=20; Max Pool Size=100;";

            try
            {

                // Demo 1: Fetch Size and Row Size

                //Create a connection to Oracle
                OracleConnection con = new OracleConnection();
                con.ConnectionString = conString;
                con.Open();

                // Create a command within the context of the connection
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from employees";
                cmd.AddToStatementCache = false;

                // Execute the command and set the FetchSize to 1 row on the DataReader
                DateTime start_time = DateTime.Now;

                for (int i = 0; i < 100; i++)
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader.FetchSize = cmd.RowSize * 1;
                    while (reader.Read())
                        ;
                    reader.Dispose();
                }

                DateTime end_time = DateTime.Now;
                TimeSpan ts = end_time - start_time;
                double ts1 = Math.Round(ts.TotalSeconds, 3);

                Console.WriteLine("Fetch Size = 1: " + ts1 + " seconds");

                // Execute the command and set the FetchSize to 100 row on the DataReader
                start_time = DateTime.Now;

                for (int i = 0; i < 100; i++)
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader.FetchSize = cmd.RowSize * 100;
                    while (reader.Read())
                        ;
                    reader.Dispose();
                }

                end_time = DateTime.Now;
                ts = end_time - start_time;
                double ts2 = Math.Round(ts.TotalSeconds, 3);

                Console.WriteLine("Fetch Size = 100: " + ts2 + " seconds");
                Console.WriteLine();

                Console.WriteLine("Percent difference: " +
                  Math.Round(((ts1 - ts2) / ts2) * 100, 2) + "%");
                Console.WriteLine("Press 'Enter' to continue");
                Console.ReadLine();

                // Demo 3: Passing Array Parameters
                // Let's pass array parameters between .NET and Oracle
                // This stored procedure takes an input array and copies its
                // values to the output array
                cmd = new OracleCommand("MYPACK.MYSP", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // The arrays contain values of data type Varchar2
                // Then bind the arrays
                OracleParameter param1 = cmd.Parameters.Add("param1",
                  OracleDbType.Varchar2);
                OracleParameter param2 = cmd.Parameters.Add("param2",
                  OracleDbType.Varchar2);
                param1.CollectionType =
                  OracleCollectionType.PLSQLAssociativeArray;
                param2.CollectionType =
                  OracleCollectionType.PLSQLAssociativeArray;

                //Setup the parameter direction
                //Note that param2 is NULL
                param1.Direction = ParameterDirection.Input;
                param2.Direction = ParameterDirection.Output;
                param1.Value = new string[3] { "Oracle", "Code", "2018" };
                param2.Value = null;

                //Specify the maximum number of elements in the arrays
                // and the maximum size of the varchar2
                param1.Size = 3;
                param2.Size = 3;
                param1.ArrayBindSize = new int[3] { 20, 20, 20 };
                param2.ArrayBindSize = new int[3] { 20, 20, 20 };

                //Execute the statement and output the results
                cmd.ExecuteNonQuery();
                for (int i = 0; i < 3; i++)
                {
                    Console.Write((param2.Value as OracleString[])[i]);
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Press 'Enter' to continue");
                Console.ReadLine();

                // Demo 4: Batch SQL, REF Cursors
                // Anonymous PL/SQL block embedded in code - executes in one 
                // DB round trip
                 
                string cmdtxt = 
                  "BEGIN " +
                  "OPEN :1 for select first_name,department_id from employees where department_id = 10; " +
                  "OPEN :2 for select first_name,department_id from employees where department_id = 20; " +
                  "OPEN :3 for select first_name,department_id from employees where department_id = 30; " +
                  "END;";

                cmd = new OracleCommand(cmdtxt, con);
                cmd.CommandType = CommandType.Text;

                // ODP.NET has native Oracle data types, such as Oracle REF 
                // Cursors, which can be mapped to .NET data types

                //Bind REF Cursor Parameters for each department
                //Select employees in department 10
                OracleParameter p1 = cmd.Parameters.Add("refcursor1",
                  OracleDbType.RefCursor);
                p1.Direction = ParameterDirection.Output;

                //Select employees in department 20
                OracleParameter p2 = cmd.Parameters.Add("refcursor2",
                  OracleDbType.RefCursor);
                p2.Direction = ParameterDirection.Output;

                //Select employees in department 30
                OracleParameter p3 = cmd.Parameters.Add("refcursor3",
                  OracleDbType.RefCursor);
                p3.Direction = ParameterDirection.Output;

                //Execute batched statement
                
                OracleDataReader dr1 = cmd.ExecuteReader();

                while (dr1.Read())
                {
                    Console.WriteLine("Employee Name: " + dr1.GetString(0) + ", " +
                      "Salary:" + dr1.GetDecimal(1));
                }
                Console.WriteLine();

                // Get the DataReader for the next REF Cursor
                dr1.NextResult();
                while (dr1.Read())
                {
                    Console.WriteLine("Employee Name: " + dr1.GetString(0) + ", " +
                      "Salary:" + dr1.GetDecimal(1));
                }
                dr1.NextResult();
                while (dr1.Read())
                {
                    Console.WriteLine("Employee Name: " + dr1.GetString(0) + ", " +
                      "Salary:" + dr1.GetDecimal(1));
                }

               
                Console.WriteLine("Press 'Enter' to continue");
                Console.ReadLine();


            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
