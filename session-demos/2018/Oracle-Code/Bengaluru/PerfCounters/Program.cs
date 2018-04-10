using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Oracle.ManagedDataAccess.Client;

namespace Connections_and_Bind_Variables
{
    class Program
    {
        static void Main(string[] args)
        {
            string conString = "User Id=hr;Password=<password>;Data Source=oracle;" +
                "Pooling = true; Min Pool Size = 20; Max Pool Size = 100;";

            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    // infinite loop to show performance counters stats

                    while (true)
                    {
                        con.Open();

                        using (OracleCommand cmd = con.CreateCommand())
                        {
                            cmd.BindByName = true;

                            //Use the command to display employee names from 
                            // the EMPLOYEES table for bind variable 'id'
                            cmd.CommandText = "select first_name from employees where department_id = :id";

                            // Assign id to the department number 50 
                            OracleParameter id = new OracleParameter("id", 50);
                            cmd.Parameters.Add(id);

                            //Execute the command and read data
                            OracleDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                // Do work
                            }
                            reader.Dispose();
                        }

                        con.Close();
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
