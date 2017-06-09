using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Callen
{
    public class DBConnect // Used to create a connection with the data base
    {
        public static SqlConnection getConnection()
        {
            //Local server
            return new SqlConnection("Data Source=XDYEPC\\SQLEXPRESS;Initial Catalog=G_Callen;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            //UA server
            //return new SqlConnection("Data Source = tcp: 193.136.175.33\\SQLSERVER2012,8293;Initial Catalog = p1g10; uid = ;password = ");
        }
    }
}
