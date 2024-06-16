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
    public partial class HoaDonFrm : Form
    {
        List<HoaDonBan> listHoaDon = new List<HoaDonBan>();
        HoaDonBan hoaDonChon = null;
        public HoaDonFrm()
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
            string query = "SELECT * FROM tb_invoices";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                HoaDonBan hoaDon = new HoaDonBan();
                hoaDon.id = reader.GetInt64(0);
                hoaDon.total = reader.GetInt64(1);
                hoaDon.date = reader.GetDateTime(2);
                hoaDon.customer_id = reader.GetInt64(3);
                hoaDon.employee_id = reader.GetInt64(4);
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
            dt.Columns.Add("Ngày lập");
            dt.Columns.Add("Khách hàng");
            dt.Columns.Add("Nhân viên");
            foreach (HoaDonBan hoaDon in listHoaDon)
            {
                dt.Rows.Add(hoaDon.id, hoaDon.total, hoaDon.date, hoaDon.customer_id, hoaDon.employee_id);
            }
            dgvHoaDon.DataSource = dt;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (hoaDonChon == null)
            {
                MessageBox.Show("Chưa chọn hóa");
                return;
            }
            if (deleteInvoice(hoaDonChon.id))
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

        public bool deleteInvoice(long id)
        {
            if (!deleteDetailInvoice(hoaDonChon.id))
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
            string query = "DELETE FROM tb_invoices WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result > 0;

        }

        public bool deleteDetailInvoice(long id)
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return false;
            }
            string query = "DELETE FROM tb_detail_order WHERE invoice_id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result > 0;
        }

        public List<ChiTietHoaDonBan> getDetailInvoice(long id)
        {
            List<ChiTietHoaDonBan> list = new List<ChiTietHoaDonBan>();
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối không thành công");
                return null;
            }
            string query = "SELECT * FROM tb_detail_order WHERE invoice_id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ChiTietHoaDonBan chiTiet = new ChiTietHoaDonBan();
                chiTiet.id = reader.GetInt64(0);
                chiTiet.product_id = reader.GetInt64(1);
                chiTiet.invoice_id = reader.GetInt64(1);
                chiTiet.price = reader.GetInt64(3);
                chiTiet.quantity = reader.GetInt32(4);
                chiTiet.total = reader.GetInt64(5);
                list.Add(chiTiet);
            }
            con.Close();
            return list;
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

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (hoaDonChon == null)
            {
                MessageBox.Show("Chưa chọn hóa đơn");
                return;
            }   
            ChiTietHoaDonBanFrm chiTietForm = new ChiTietHoaDonBanFrm();
            chiTietForm.addInvoice(hoaDonChon);
            chiTietForm.ShowDialog();
        }
    }
}
