using QuanLiBanHang.Entity;
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
    public partial class KhachHangFrm : Form
    {
        List<KhachHang> listKhachHang = new List<KhachHang>();
        public KhachHangFrm()
        {
            InitializeComponent();
            readData();
        }

        public void refesh()
        {
            txtMa.Text = "";
            txtTen.Text = "";
            txtDiaChi.Text = "";
            txtTP.Text = "";
            txtSDT.Text = "";
        }

        public void readData()
        {
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
            String query = "SELECT * FROM tb_customer";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            listKhachHang.Clear();
            while (reader.Read())
            {
                KhachHang khachHang = new KhachHang();
                khachHang.id = reader.GetInt64(0);
                khachHang.name = reader.GetString(1);
                khachHang.address = reader.GetString(2);
                khachHang.city = reader.GetString(3);
                khachHang.phone = reader.GetString(4);
                listKhachHang.Add(khachHang);
            }
            con.Close();
            loadData();
        }

        public void loadData()
        {
            
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã KH");
            dt.Columns.Add("Họ và Tên");
            dt.Columns.Add("Số điện thoại");
            dt.Columns.Add("Địa chỉ");
            dt.Columns.Add("Thành phố");
        

            foreach (KhachHang khachHang in listKhachHang)
            {
                dt.Rows.Add(khachHang.id, khachHang.name, khachHang.phone, khachHang.address, khachHang.city);
            }
            dgvKhachHang.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtTen.Text == "" || txtDiaChi.Text == "" || txtTP.Text == "" || txtSDT.Text == "")
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
            
            String query = "INSERT INTO tb_customer VALUES(@id, @name, @address, @city, @phone)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", DateTime.Now.Ticks / 1000000);
            cmd.Parameters.AddWithValue("@name", txtTen.Text);
            cmd.Parameters.AddWithValue("@address", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@city", txtTP.Text);
            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0) {
                MessageBox.Show("Thêm thất bại");
                return;
            }
            MessageBox.Show("Thêm thành công");
            readData();
            refesh();
        }

       

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMa.Text == "")
            {
                MessageBox.Show("Vui lòng chọn thể loại cần xóa");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            
            String query = "DELETE FROM tb_customer WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", txtMa.Text);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Xóa thất bại");
                return;
            }
            MessageBox.Show("Xóa thành công");
            readData();
            refesh();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtTen.Text == "" || txtDiaChi.Text == "" || txtSDT.Text == "" || txtTP.Text == "")
            {
                MessageBox.Show("Vui lòng chọn thể loại cần sửa");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "UPDATE tb_customer SET name = @name, address = @address, city = @city, phone = @phone WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", txtTen.Text);
            cmd.Parameters.AddWithValue("@address", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@city", txtTP.Text);
            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);
            cmd.Parameters.AddWithValue("@id", txtMa.Text);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Sửa thất bại");
                return;
            }
            MessageBox.Show("Sửa thành công");
            readData();
            refesh();
        }

        private void dgvKhachHang_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            KhachHang khachHang = listKhachHang[index];
            txtMa.Text = khachHang.id.ToString();
            txtTen.Text = khachHang.name;
            txtDiaChi.Text = khachHang.address;
            txtSDT.Text = khachHang.phone;
            txtTP.Text = khachHang.city;
        }
    }
}
