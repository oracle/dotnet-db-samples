using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace FetchSize_Batching_and_RCs
{
    class Program
    {
        static void Main(string[] args)
        {
            string conString = "User Id=hr;Password=<password>;Data Source=oracle;";

            try
            {

                // Demo 1: Fetch Size and Row Size
                // Let's see the performance difference when we set the FetchSize
                // differently for the same SQL statement 
                // You should see significant performance improvement as number
                // of round trips decrease with increased FetchSize.

                //Create a connection to Oracle
                OracleConnection con = new OracleConnection();
                con.ConnectionString = conString;
                con.Open();

                // Create a command within the context of the connection
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from employees";
                cmd.AddToStatementCache = false;

                // Execute the command and set the FetchSize to 1 row
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

                // Execute the command and set the FetchSize to 107 rows
                start_time = DateTime.Now;

                for (int i = 0; i < 100; i++)
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader.FetchSize = cmd.RowSize * 107;
                    while (reader.Read())
                        ;
                    reader.Dispose();
                }

                end_time = DateTime.Now;
                ts = end_time - start_time;
                double ts2 = Math.Round(ts.TotalSeconds, 3);

                Console.WriteLine("Fetch Size = 107: " + ts2 + " seconds");
                Console.WriteLine();

                Console.WriteLine("Percent difference: " +
                  Math.Round(((ts1 - ts2) / ts2) * 100, 2) + "%");
                Console.WriteLine("Press 'Enter' to continue");
                Console.ReadLine();


                // Demo 2: Batch SQL, REF Cursors, and MARS
                // Anonymous PL/SQL block embedded in code - executes in one 
                // DB round trip
                // And why don't we try out MARS in Oracle as well 
                string cmdtxt = "DECLARE a NUMBER:= 20; " +
                  "BEGIN " +
                  "OPEN :1 for select first_name,department_id from employees where department_id = 10; " +
                  "OPEN :2 for select first_name,department_id from employees where department_id = a; " +
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
                cmd.ExecuteNonQuery();

                // Let's retrieve data from the 2nd and 3rd parameter without 
                // having to fetch results from the first parameter
                // At the same time, we'll test MARS with Oracle
                OracleDataReader dr1 =
                  ((OracleRefCursor)cmd.Parameters[2].Value).GetDataReader();
                OracleDataReader dr2 =
                  ((OracleRefCursor)cmd.Parameters[1].Value).GetDataReader();

                // Let's retrieve both DataReaders at one time to test if 
                // MARS works
                while (dr1.Read() && dr2.Read())
                {
                    Console.WriteLine("Employee Name: " + dr1.GetString(0) + ", " +
                      "Employee Dept:" + dr1.GetDecimal(1));
                    Console.WriteLine("Employee Name: " + dr2.GetString(0) + ", " +
                      "Employee Dept:" + dr2.GetDecimal(1));
                    Console.WriteLine();
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
