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
            return new SqlConnection("Data Source=XDYEPC\\SQLEXPRESS;Initial Catalog=G_Callen;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
