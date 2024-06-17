using Microsoft.Win32;
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
    public partial class NhanVienFrm : Form
    {
        List<NhanVien> listNhanVien = new List<NhanVien>();
        List<ChucVu> listChucVu = new List<ChucVu>();
        public NhanVienFrm()
        {
            InitializeComponent();
            loadDataChucVu();
            readData();
        }

        public void refesh()
        {
            
            txtMa.Text = "";
            txtHoTen.Text = "";
            txtSDT.Text = "";
            txtMatKhau.Text = "";
            txtDiaChi.Text = "";
            txtNgaySinh.Text = "";
            cbxGioiTinh.SelectedIndex = -1;
            txtTP.Text = "";
            cbxChucVu.SelectedIndex = -1;
        }

        public void readData()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (ConnectDB.open() == false)
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "SELECT * FROM tb_employee";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();

           listNhanVien.Clear();
           
            while (reader.Read())
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

                listNhanVien.Add(nhanVien);
            }
            con.Close();
            loadData(); 
        }

        public void loadData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã");
            dt.Columns.Add("Họ và Tên");
            dt.Columns.Add("Số điện thoại");
            dt.Columns.Add("Mật khẩu");
            dt.Columns.Add("Địa chỉ");
            dt.Columns.Add("Ngày sinh");
            dt.Columns.Add("Giới tính");
            dt.Columns.Add("Thành phố");
            dt.Columns.Add("Chức vụ");

            foreach (NhanVien nhanVien in listNhanVien)
            {

                dt.Rows.Add(nhanVien.id, nhanVien.name, 
                    nhanVien.phone, nhanVien.password, 
                    nhanVien.address, nhanVien.date_of_birth.ToString("dd/MM/yyyy"), 
                    nhanVien.gender == 0 ? "Nữ" : "Nam", nhanVien.city, getChucVu(nhanVien.role_id));
            }

            dgvNhanVien.DataSource = dt;
        }

        public string getChucVu(long role_id)
        {
            foreach (ChucVu chucVu in listChucVu)
            {
                if (chucVu.id == role_id)
                {
                    return chucVu.name;
                }
            }
            return "";
        }

        public void loadDataChucVu()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (ConnectDB.open() == false)
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "SELECT * FROM tb_role";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ChucVu chucVu = new ChucVu();
                chucVu.id = reader.GetInt64(0);
                chucVu.name = reader.GetString(1);
                listChucVu.Add(chucVu);
                cbxChucVu.Items.Add(chucVu.name);
            }
            con.Close();
        }
        private void dgvNhanVien_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dgvNhanVien.Rows[index];
            txtMa.Text = row.Cells[0].Value.ToString();
            txtHoTen.Text = row.Cells[1].Value.ToString();
            txtSDT.Text = row.Cells[2].Value.ToString();
            txtMatKhau.Text = row.Cells[3].Value.ToString();
            txtDiaChi.Text = row.Cells[4].Value.ToString();
            txtNgaySinh.Text = row.Cells[5].Value.ToString();
            cbxGioiTinh.SelectedIndex = row.Cells[6].Value.ToString() == "Nam" ? 1 : 0;
            txtTP.Text = row.Cells[7].Value.ToString();
            for (int i = 0; i < listChucVu.Count; i++)
            {
                if (listChucVu[i].name == row.Cells[8].Value.ToString())
                {
                    cbxChucVu.SelectedIndex = i;
                    break;
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtHoTen.Text == "" || txtSDT.Text == "" || txtMatKhau.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (ConnectDB.open() ==  false)
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            
            String query = "INSERT INTO tb_employee VALUES(@id, @name, @phone, @password, @address, @date_of_birth, @gender, @city, @role_id)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", DateTime.Now.Ticks / 1000000);
            cmd.Parameters.AddWithValue("@name", txtHoTen.Text);
            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);
            cmd.Parameters.AddWithValue("@password", txtMatKhau.Text);
            cmd.Parameters.AddWithValue("@address", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@date_of_birth", DateTime.Parse(txtNgaySinh.Text));
            cmd.Parameters.AddWithValue("@gender", cbxGioiTinh.SelectedIndex);
            cmd.Parameters.AddWithValue("@city", txtTP.Text);
            cmd.Parameters.AddWithValue("@role_id", listChucVu[cbxChucVu.SelectedIndex].id);
            
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
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn xóa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.No)
            {
                return;
            }
            if (txtMa.Text == "")
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa");
                return;
            }
            long id = long.Parse(txtMa.Text);
            SqlConnection con = ConnectDB.getConnect();
            if (ConnectDB.open() == false)
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "DELETE FROM tb_employee WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            
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
            if (txtHoTen.Text == "" || txtSDT.Text == "" || txtMatKhau.Text == "" || txtDiaChi.Text == "" ||  txtNgaySinh.Text == "" || txtTP.Text == "")
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa");
                return;
            }
            if (cbxGioiTinh.SelectedIndex == -1 || cbxChucVu.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn giới tính hoặc chức vụ");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (ConnectDB.open() == false)
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "UPDATE tb_employee SET name = @name, phone = @phone, password = @password, address = @address, date_of_birth = @date_of_birth, gender = @gender, city = @city, role_id = @role_id WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", txtHoTen.Text);
            cmd.Parameters.AddWithValue("@phone", txtSDT.Text);
            cmd.Parameters.AddWithValue("@password", txtMatKhau.Text);
            cmd.Parameters.AddWithValue("@address", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@date_of_birth", DateTime.Parse(txtNgaySinh.Text));
            cmd.Parameters.AddWithValue("@gender", cbxGioiTinh.SelectedIndex);
            cmd.Parameters.AddWithValue("@city", txtTP.Text);
            cmd.Parameters.AddWithValue("@role_id", listChucVu[cbxChucVu.SelectedIndex].id);
            cmd.Parameters.AddWithValue("@id", txtMa.Text);
            if (cmd.ExecuteNonQuery() <= 0)
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
