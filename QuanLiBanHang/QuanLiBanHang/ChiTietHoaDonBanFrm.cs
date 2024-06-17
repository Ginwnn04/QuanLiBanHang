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
    public partial class ChiTietHoaDonBanFrm : Form
    {
        List<ChiTietHoaDonBan> listChiTiet = new List<ChiTietHoaDonBan>();
        HoaDonBan hoaDon;
        public ChiTietHoaDonBanFrm()
        {
            InitializeComponent();
        }

        public void addInvoice(HoaDonBan hoaDon)
        {
            this.hoaDon = hoaDon;
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
            string query = "SELECT * FROM tb_detail_order WHERE invoice_id = @invoice_id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@invoice_id", hoaDon.id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ChiTietHoaDonBan chiTiet = new ChiTietHoaDonBan();
                chiTiet.id = reader.GetInt64(0);
                chiTiet.product_id = reader.GetInt64(1);
                chiTiet.price = reader.GetInt64(2);
                chiTiet.invoice_id = reader.GetInt64(3);
                chiTiet.quantity = reader.GetInt32(4);
                chiTiet.total = reader.GetInt64(5);
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
            foreach (ChiTietHoaDonBan chiTiet in listChiTiet)
            {
                dt.Rows.Add(chiTiet.id, chiTiet.product_id, chiTiet.quantity, chiTiet.price, chiTiet.total);
            }
            dgvChiTiet.DataSource = dt;

            lbHoaDon.Text = hoaDon.id + "";
            lbNhanVien.Text = getNameNhanVien(hoaDon.employee_id);
            lbKhachHang.Text = getNameKhachHang(hoaDon.customer_id);
            lbNgay.Text = hoaDon.date + "";
            lbTien.Text = hoaDon.total + "";
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

        public string getNameKhachHang(long id)
        {
            string name = "";
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return name;
            }
            string query = "SELECT name FROM tb_customer WHERE id = @id";
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
