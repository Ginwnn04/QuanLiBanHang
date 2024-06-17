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
    public partial class HoaDonMuaFrm : Form
    {
        List<HoaDonMua> listHoaDon = new List<HoaDonMua>();
        HoaDonMua hoaDonChon = null;
        public HoaDonMuaFrm()
        {
            InitializeComponent();
            readDataHoaDon();
        
        }
        public void readDataHoaDon()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return;
            }
            string query = "SELECT * FROM tb_import";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                HoaDonMua hoaDon = new HoaDonMua();
                hoaDon.id = reader.GetInt64(0);
                hoaDon.total = reader.GetInt64(1);
                hoaDon.suplier_id = reader.GetInt64(2);
                hoaDon.employee_id = reader.GetInt64(3);
                hoaDon.date = reader.GetDateTime(4);
                listHoaDon.Add(hoaDon);
            }
            con.Close();
            loadDataHoaDon();
        }

        public void loadDataHoaDon()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã hóa đơn");
            dt.Columns.Add("Tổng tiền");
            dt.Columns.Add("Nhân viên");
            dt.Columns.Add("Nhà cung cấp");
            dt.Columns.Add("Ngày lập");
            foreach (HoaDonMua hoaDon in listHoaDon)
            {
                dt.Rows.Add(hoaDon.id, hoaDon.total, getNameNhanVien(hoaDon.employee_id), getNameNhaCungCap(hoaDon.suplier_id), hoaDon.date);
            }
            dgvHoaDon.DataSource = dt;
        }

        private void dgvHoaDon_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            if (index == -1 || index >= listHoaDon.Count)
            {
                hoaDonChon = null;
                return;
            }
            hoaDonChon = listHoaDon[index];
        }

        public bool deleteImport(long id)
        {
            if (!deleteDetailImport(hoaDonChon.id))
            {
                MessageBox.Show("Xóa chi tiết hóa đơn không thành công");
                return false;
            }

            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return false;
            }
            string query = "DELETE FROM tb_import WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result > 0;

        }

        public bool deleteDetailImport(long id)
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return false;
            }
            string query = "DELETE FROM tb_detail_import WHERE import_id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result > 0;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn xóa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.No)
            {
                return;
            }
            if (hoaDonChon == null)
            {
                MessageBox.Show("Chưa chọn hóa");
                return;
            }
            if (deleteImport(hoaDonChon.id))
            {
                MessageBox.Show("Xóa thành công");
                listHoaDon.Remove(hoaDonChon);
                loadDataHoaDon();
            }
            else
            {
                MessageBox.Show("Xóa không thành công");
            }
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (hoaDonChon == null)
            {
                MessageBox.Show("Chưa chọn hóa đơn");
                return;
            }
            ChiTietHoaDonMuaFrm chiTietHoaDonMuaFrm = new ChiTietHoaDonMuaFrm();
            chiTietHoaDonMuaFrm.addImport(hoaDonChon);
            chiTietHoaDonMuaFrm.ShowDialog();

        }

        private void dgvHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public string getNameNhanVien(long id)
        {
            string name = "";
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return name;
            }
            string query = "SELECT name FROM tb_employee WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                name = reader.GetString(0);
            }
            con.Close();
            return name;

        }

        public string getNameNhaCungCap(long id)
        {
            string name = "";
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return name;
            }
            string query = "SELECT name FROM tb_suplier WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                name = reader.GetString(0);
            }
            con.Close();
            return name;
        }
    }
}
