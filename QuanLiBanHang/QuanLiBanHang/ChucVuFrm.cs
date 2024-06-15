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
using static System.Net.WebRequestMethods;

namespace QuanLiBanHang
{
    public partial class ChucVuFrm : Form
    {
        List<ChucVu> listChucVu = new List<ChucVu>();
        public ChucVuFrm()
        {
            InitializeComponent();
            readData();
        }
        public void refesh()
        {
            txtMa.Text = "";
            txtTen.Text = "";
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
            String query = "SELECT * FROM tb_role";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            listChucVu.Clear();
            while (reader.Read())
            {
                ChucVu chucVu = new ChucVu();
                chucVu.id = reader.GetInt64(0);
                chucVu.name = reader.GetString(1);
                listChucVu.Add(chucVu);
            }
            con.Close();
            loadData();
        }

        public void loadData()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã chức vụ");
            dt.Columns.Add("Tên chức vụ");
   
            foreach (ChucVu chucVu in listChucVu)
            {
                dt.Rows.Add(chucVu.id, chucVu.name);
            }
            dgvChucVu.DataSource = dt;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtTen.Text == "")
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

            String query = "INSERT INTO tb_role VALUES(@id, @name)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", DateTime.Now.Ticks / 1000000);
            cmd.Parameters.AddWithValue("@name", txtTen.Text);

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

            String query = "DELETE FROM tb_role WHERE id = @id";
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
            if (txtTen.Text == "")
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
            String query = "UPDATE tb_role SET name = @name WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", txtTen.Text);
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

        private void dgvChucVu_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            ChucVu chucVu = listChucVu[index];
            txtMa.Text = chucVu.id.ToString();
            txtTen.Text = chucVu.name;
        }

       
    }
}
