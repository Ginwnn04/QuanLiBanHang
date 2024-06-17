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
    public partial class ChiTietHoaDonMuaFrm : Form
    {
        List<ChiTietHoaDonMua> listChiTiet = new List<ChiTietHoaDonMua>();
        HoaDonMua donMua = null;
        public ChiTietHoaDonMuaFrm()
        {
            InitializeComponent();
        }

        public void addImport(HoaDonMua hoaDon)
        {
            this.donMua = hoaDon;
            readDataChiTietHoaDon();
        }

        public void readDataChiTietHoaDon()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return;
            }
            string query = "SELECT * FROM tb_detail_import WHERE import_id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", donMua.id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ChiTietHoaDonMua chiTiet = new ChiTietHoaDonMua();
                chiTiet.id = reader.GetInt64(0);
                chiTiet.product_id = reader.GetInt64(1);
                chiTiet.price = reader.GetInt64(2);
                chiTiet.quantity = reader.GetInt32(3);
                chiTiet.total = reader.GetInt64(4);
                chiTiet.import_id = reader.GetInt64(5);
                listChiTiet.Add(chiTiet);
            }
            con.Close();
            loadDataChiTietHoaDon();
        }

        public void loadDataChiTietHoaDon()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã chi tiết");
            dt.Columns.Add("Tên sản phẩm");
            dt.Columns.Add("Số lượng");
            dt.Columns.Add("Đơn giá");
            dt.Columns.Add("Thành tiền");
            foreach (ChiTietHoaDonMua chiTiet in listChiTiet)
            {
                dt.Rows.Add(chiTiet.id, chiTiet.product_id, chiTiet.quantity, chiTiet.price, chiTiet.total);
            }
            dgvChiTiet.DataSource = dt;

            lbHoaDon.Text = donMua.id + "";
            lbNhanVien.Text = getNameNhanVien(donMua.employee_id);
            lbNhaCungCap.Text = getNameNhaCungCap(donMua.suplier_id);
            lbNgay.Text = donMua.date + "";
            lbTien.Text = donMua.total + "";
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
