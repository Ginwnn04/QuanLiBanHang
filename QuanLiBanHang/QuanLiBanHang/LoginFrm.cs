using QuanLiBanHang.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            // Anti injection sql
            String query = "SELECT * FROM tb_employee WHERE phone = @phone AND password = @password";

            // inject sql
            // String query1 = "SELECT * FROM tb_employee WHERE phone = " + " '" + txtSDT.Text + "' " + " AND password = " + " '" + txtPassword.Text + "' ";

            SqlCommand cmd = new SqlCommand(query, con);

            

            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);
            cmd.Parameters.AddWithValue("@password", txtPassword.Text);

            SqlDataReader reader = cmd.ExecuteReader();

            // Đăng nhập đúng
            if (reader.Read())
            {
                NhanVien nhanVien = new NhanVien();
                nhanVien.id = reader.GetInt64(0);
                nhanVien.name = reader.GetString(1);
                nhanVien.phone = reader.GetString(2);
                nhanVien.password = reader.GetString(3);
                nhanVien.address = reader.GetString(4);
                nhanVien.date_of_birth = reader.GetDateTime(5);
                nhanVien.gender = reader.GetInt32(6);
                nhanVien.city = reader.GetString(7);
                nhanVien.role_id = reader.GetInt64(8);

               
                //
                NhanVien.nhanVienLogin = nhanVien;
                MenuFrm menu = new MenuFrm();
                menu.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu");
            }
            con.Close();
        }
    }
}
