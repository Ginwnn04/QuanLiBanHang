using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang
{
    public class ConnectDB
    {
        private static String strConnect = "Data Source=DESKTOP-6VVP7BU;Initial Catalog=QuanLiBanHang;Integrated Security=True";
        public static SqlConnection con = null;

        private ConnectDB()
        {

        }

        public static SqlConnection getConnect()
        {
            if (con == null)
            {
                con = new SqlConnection(strConnect);
            }
            return con;
        }

        public static bool open()
        {
            if (con == null)
            {
                return false;
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
                return true;
            }
            return true;
        }
    }
}
