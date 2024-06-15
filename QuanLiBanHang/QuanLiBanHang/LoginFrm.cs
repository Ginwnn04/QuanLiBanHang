using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiBanHang
{
    public partial class LoginFrm : Form
    {
        public LoginFrm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (txtSDT.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }

            SqlConnection con = ConnectDB.getConnect();
            if (con == null)
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            String query = "SELECT * FROM tb_employee WHERE is_deleted = 0";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (txtSDT.Text == reader.GetString(3) && txtPassword.Text == reader.GetString(4))
                {
                    MessageBox.Show("Đăng nhập thành công");

                }
                else
                {
                    MessageBox.Show("Sai thông tin đăng nhập");
                }
            }
        }
    }
}
