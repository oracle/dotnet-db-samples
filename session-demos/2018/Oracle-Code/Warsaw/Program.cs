using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace ConnectionsBind
{
    class Program
    {
        static void Main(string[] args)
        {      

            string constr = "User Id=hr;Password=hr;Data Source=localhost:1521/orcl;" +
                   "Pooling=true;Min Pool Size=20; Max Pool Size=100;";

            OracleConnection con = new OracleConnection(constr);
            con.Open();

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("select    country_name ");
            sbSQL.Append("from      countries ");
            sbSQL.Append("where     country_id = :country_id");

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText = sbSQL.ToString();

            OracleParameter p_country_id = new OracleParameter();
            p_country_id.OracleDbType = OracleDbType.Varchar2;
            p_country_id.Value = "UK";

            cmd.Parameters.Add(p_country_id);

            OracleDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Console.WriteLine("Country Name: {0}", dr.GetOracleString(0));
            }

            Console.WriteLine("Press 'Enter' to continue");
            Console.ReadLine();

            dr.Dispose();
            p_country_id.Dispose();
            cmd.Dispose();
            con.Dispose();
        }
    }
}
