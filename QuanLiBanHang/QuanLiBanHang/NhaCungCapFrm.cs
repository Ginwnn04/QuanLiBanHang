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
    public partial class NhaCungCapFrm : Form
    {
        List<NhaCungCap> listNhaCungCap = new List<NhaCungCap>();
        public NhaCungCapFrm()
        {
            InitializeComponent();
            readData();
        }

        public void refesh()
        {
            txtMa.Text = "";
            txtTen.Text = "";
            txtSDT.Text = "";
            txtDiaChi.Text = "";
            txtTP.Text = "";
        }

        public void readData()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            
            String query = "SELECT * FROM tb_suplier";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            listNhaCungCap.Clear();
            while (reader.Read())
            {
                NhaCungCap nhaCungCap = new NhaCungCap();
                nhaCungCap.id = reader.GetInt64(0);
                nhaCungCap.name = reader.GetString(1);
                nhaCungCap.address = reader.GetString(2);
                nhaCungCap.phone = reader.GetString(3);
                nhaCungCap.city = reader.GetString(4);
                listNhaCungCap.Add(nhaCungCap);
            }
            con.Close();
            loadData();
        }

        public void loadData()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã");
            dt.Columns.Add("Tên nhà cung cấp");
            dt.Columns.Add("Địa chỉ");
            dt.Columns.Add("SĐT");
            dt.Columns.Add("Thành phố");

           
            foreach (NhaCungCap nhaCungCap in listNhaCungCap)
            {
                dt.Rows.Add(nhaCungCap.id, nhaCungCap.name, nhaCungCap.address, nhaCungCap.phone, nhaCungCap.city);
            }
            dgvNhaCungCap.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtTen.Text == "" || txtDiaChi.Text == "" || txtSDT.Text == "" || txtTP.Text == "")
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
            String query = "INSERT INTO tb_suplier VALUES(@id, @name, @address, @phone, @city)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", DateTime.Now.Ticks / 1000000);
            cmd.Parameters.AddWithValue("@name", txtTen.Text);
            cmd.Parameters.AddWithValue("@address", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);
            cmd.Parameters.AddWithValue("@city", txtTP.Text);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Thêm thất bại");
                return;
            }
            MessageBox.Show("Thêm thành công");
            readData();
            refesh();
        }

        private void dgvNhaCungCap_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            NhaCungCap nhaCungCap = listNhaCungCap[index];
            txtMa.Text = nhaCungCap.id.ToString();
            txtTen.Text = nhaCungCap.name;
            txtDiaChi.Text = nhaCungCap.address;
            txtSDT.Text = nhaCungCap.phone;
            txtTP.Text = nhaCungCap.city;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn xóa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.No)
            {
                return;
            }
            if (txtMa.Text == "")
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "DELETE FROM tb_suplier WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", txtMa.Text);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result == 0) {
                MessageBox.Show("Xóa thất bại");
                return;
            }
            MessageBox.Show("Xóa thành công");
            readData();
            refesh();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtTen.Text == "" || txtDiaChi.Text == "" || txtSDT.Text == "")
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "UPDATE tb_suplier SET name = @name, address = @address, phone = @phone, city = @city WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", txtTen.Text);
            cmd.Parameters.AddWithValue("@address", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);
            cmd.Parameters.AddWithValue("@city", txtTP.Text);
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
    }
}
