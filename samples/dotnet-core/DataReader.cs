using System;
using Oracle.ManagedDataAccess.Client;

namespace ODPCoreDataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Demo: Basic ODP.NET Core application
            //Create a connection to Oracle			
            string conString = "User Id=hr;Password=<password>;" +

            //How to connect to an Oracle DB without SQL*Net configuration 
            //  file also known as tnsnames.ora.
            "Data Source=<ip or hostname>:1521/<service name>;";

            //How to connect to an Oracle DB with a DB alias.
            //Uncomment below and comment above.
            //"Data Source=<service name alias>;";

            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString;
            con.Open();

            //Create a command within the context of the connection
            //Use the command to display employee names from 
            // the EMPLOYEES table
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "select first_name from employees where department_id = 50";

            //Execute the command and use DataReader to display the data
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Employee Name: " + reader.GetString(0));
            }

            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to continue");
            Console.ReadLine();
        }
    }
}
