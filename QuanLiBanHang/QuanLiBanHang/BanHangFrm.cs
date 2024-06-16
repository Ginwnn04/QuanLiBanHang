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
    public partial class BanHangFrm : Form
    {
        List<SanPham> listSanPham = new List<SanPham>();
        List<ChiTietHoaDonBan> listChiTietDonBan = new List<ChiTietHoaDonBan>();
        long total = 0;
        long idChiTietChon = -1;
        ChiTietHoaDonBan chiTietChon = new ChiTietHoaDonBan();

        public BanHangFrm()
        {
            InitializeComponent();
            loadDataSanPhamChon();
            readDataSanPham();
        }

        public void loadDataSanPhamChon()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã chi tiết");
            dt.Columns.Add("Tên sản phẩm");
            dt.Columns.Add("Số lượng");
            dt.Columns.Add("Đơn giá");
            dt.Columns.Add("Thành tiền");

            total = 0;

            foreach (ChiTietHoaDonBan ct in listChiTietDonBan)
            {
                dt.Rows.Add(ct.id, getSanPham(ct.product_id).name, ct.quantity, ct.price, ct.total);
                total += ct.total;

            }
            lbTien.Text = total.ToString();
            dgvSanPham.DataSource = dt;
        }

        public SanPham getSanPham(long id)
        {
            foreach (SanPham sp in listSanPham)
            {
                if (sp.id == id)
                {
                    return sp;
                }
            }
            return null;
        }

        public void readDataSanPham()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }

            String query = "SELECT * FROM tb_product";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();

            listSanPham.Clear();
            cbxSanPham.Items.Clear();

            while (reader.Read())
            {
                SanPham sp = new SanPham();
                sp.id = reader.GetInt64(0);
                sp.name = reader.GetString(1);
                sp.quantity = reader.GetInt32(2);
                sp.price = reader.GetInt64(3);
                sp.suplier_id = reader.GetInt64(4);
                sp.price_import = reader.GetInt64(5);

                listSanPham.Add(sp);
                cbxSanPham.Items.Add(sp.name);
            }
            con.Close();
        }

        private void cbxSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (cbxSanPham.SelectedIndex == -1 || idChiTietChon != -1)
            {
                return;
            }
            lbSoLuong.Text = listSanPham[cbxSanPham.SelectedIndex].quantity.ToString();
            dudSoLuong.Text = "1";
        }

        public ChiTietHoaDonBan isExistChiTiet(long product_id)
        {
            foreach (ChiTietHoaDonBan ct in listChiTietDonBan)
            {
                if (ct.product_id == product_id)
                {
                    return ct;
                }
            }
            return null;
        }

        private void dgvSanPham_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
           
            if (e.RowIndex == -1 || e.RowIndex >= listChiTietDonBan.Count)
            {
                return;
            }
            chiTietChon = listChiTietDonBan[e.RowIndex];

            if (idChiTietChon == -1)
            {
                idChiTietChon = chiTietChon.id;
                
                
            }
            else
            {
                if (idChiTietChon != chiTietChon.id)
                {
                    idChiTietChon = chiTietChon.id;
             
                    
                }
                else
                {
                    return;
                }
            }
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            SanPham sp = getSanPham(chiTietChon.product_id);

            lbSoLuong.Text = sp.quantity.ToString();
            



            for (int i = 0; i < listSanPham.Count; i++)
            {
                if (listSanPham[i].id == chiTietChon.product_id)
                {
                    cbxSanPham.SelectedIndex = i;
                    break;
                }
            }
            dudSoLuong.Text = chiTietChon.quantity + "";
            btnDatMon.Enabled = false;
            cbxSanPham.Enabled = false;

        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (cbxSanPham.SelectedIndex == -1)
            {
                MessageBox.Show("Chưa chọn sản phẩm");
                return;
            }

            SanPham sp = listSanPham[cbxSanPham.SelectedIndex];

            if (sp.quantity < int.Parse(dudSoLuong.Text))
            {
                MessageBox.Show("Số lượng không đủ");
                return;
            }

            ChiTietHoaDonBan tmp = isExistChiTiet(sp.id);
            if (tmp == null)
            {
                ChiTietHoaDonBan ct = new ChiTietHoaDonBan();
                ct.id = DateTime.Now.Ticks / 1000000;
                ct.product_id = sp.id;
                ct.quantity = Int32.Parse(dudSoLuong.Text);
                ct.price = sp.price;
                ct.total = ct.quantity * ct.price;

                listChiTietDonBan.Add(ct);
            }
            else
            {
                tmp.quantity += int.Parse(dudSoLuong.Text);
                tmp.total = tmp.quantity * tmp.price;
            }
            loadDataSanPhamChon();
            sp.quantity -= int.Parse(dudSoLuong.Text);
            lbSoLuong.Text = sp.quantity.ToString();
            dudSoLuong.Text = "1";
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            SanPham sp = getSanPham(chiTietChon.product_id);
            sp.quantity += chiTietChon.quantity;

            chiTietChon.quantity = int.Parse(dudSoLuong.Text);
            chiTietChon.total = chiTietChon.quantity * chiTietChon.price;
            
            sp.quantity -= chiTietChon.quantity;
            lbSoLuong.Text = sp.quantity + "";

            loadDataSanPhamChon();
            MessageBox.Show("Sửa thành công");
            btnDatMon.Enabled = true;
            cbxSanPham.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            cbxSanPham.SelectedIndex = -1;
            lbSoLuong.Text = "";
            dudSoLuong.Text = "1";
            idChiTietChon = -1;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            SanPham sp = getSanPham(chiTietChon.product_id);
            sp.quantity += chiTietChon.quantity;
            listChiTietDonBan.Remove(chiTietChon);
            lbSoLuong.Text = sp.quantity + "";
            loadDataSanPhamChon();
            MessageBox.Show("Xoá thành công");
            btnDatMon.Enabled = true;
            cbxSanPham.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            cbxSanPham.SelectedIndex = -1;
            lbSoLuong.Text = "";
            dudSoLuong.Text = "1";
            idChiTietChon = -1;
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            long invocie_id = writeDataInvoice();
            if (invocie_id == -1)
            {
                MessageBox.Show("Thanh toán thất bại");
                return;
            }
            if (writeDataChiTiet(invocie_id))
            {
                MessageBox.Show("Thanh toán thành công");
            }
        }

        public bool writeDataChiTiet(long invoice_id)
        {
            if (listChiTietDonBan.Count == 0)
            {
                MessageBox.Show("Không thể thanh toán vì chưa chọn sản phẩm");
                return false;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return false;
            }
            string query = "INSERT INTO tb_detail_order VALUES (@id, @product_id, @price, @invoice_id, @quantity, @total)";
            SqlCommand cmd = new SqlCommand(query, con);
            foreach (ChiTietHoaDonBan ct in listChiTietDonBan)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id", ct.id);
                cmd.Parameters.AddWithValue("product_id", ct.product_id);
                cmd.Parameters.AddWithValue("price", ct.price);
                cmd.Parameters.AddWithValue("quantity", ct.quantity);
                cmd.Parameters.AddWithValue("total", ct.total);
                cmd.Parameters.AddWithValue("invoice_id", invoice_id);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    MessageBox.Show("Lỗi khi thêm chi tiết");
                    con.Close();
                    return false;
                }
            }
            foreach (ChiTietHoaDonBan ct in listChiTietDonBan)
            {
                SanPham sp = getSanPham(ct.product_id);

                if (!updateQuantity(ct.product_id, sp.quantity))
                {
                    MessageBox.Show("Lỗi khi cập nhật số lượng");
                    return false;
                }
            }
            return true;
        }

        public bool updateQuantity(long product_id, int newQuantity)
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return false;
            }
            string query = "UPDATE tb_product SET quantity = @quantity WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("quantity", newQuantity);
            cmd.Parameters.AddWithValue("id", product_id);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Lỗi khi cập nhật số lượng");
                return false;
            }
            return true;
        }

        public long writeDataInvoice()
        {
            HoaDonBan invoice = new HoaDonBan();
            invoice.id = DateTime.Now.Ticks / 1000000;
            invoice.employee_id = 638539669000;
            invoice.customer_id = 638540592963;
            invoice.total = total;
            invoice.date = DateTime.Now;

            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return -1;
            }
            string query = "INSERT INTO tb_invoices VALUES (@id, @total, @time, @customer_id, @employee_id)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("id", invoice.id);
            cmd.Parameters.AddWithValue("total", invoice.total);
            cmd.Parameters.AddWithValue("customer_id", invoice.customer_id);
            cmd.Parameters.AddWithValue("employee_id", invoice.employee_id);
            cmd.Parameters.AddWithValue("time", invoice.date);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Lỗi khi thêm hóa đơn");
                return -1;
            }

            return invoice.id;
        }
    }
    
}
