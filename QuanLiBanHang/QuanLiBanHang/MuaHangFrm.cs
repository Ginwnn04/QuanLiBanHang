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
    public partial class MuaHangFrm : Form
    {
        List<SanPham> listSanPham = new List<SanPham>();
        List<NhaCungCap> listNhaCungCap = new List<NhaCungCap>();
        List<ChiTietHoaDonMua> listChiTietMua = new List<ChiTietHoaDonMua>();
        long total = 0;
        
        int indexSuplierSelected = -1;
        ChiTietHoaDonMua ChiTietChon = null;

        public MuaHangFrm()
        {
            InitializeComponent();
            readDataNhaCungCap();
            loadDataSanPhamChon();
        }


        public void readDataNhaCungCap()
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open()) // 
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }

            String query = "SELECT * FROM tb_suplier";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader(); 

            while (reader.Read())
            {
                NhaCungCap ncc = new NhaCungCap();
                ncc.id = reader.GetInt64(0);
                ncc.name = reader.GetString(1);
                ncc.address = reader.GetString(2);
                ncc.phone = reader.GetString(3);
                ncc.city = reader.GetString(4);
                listNhaCungCap.Add(ncc);
                cbxNhaCungCap.Items.Add(ncc.name);
            }
            con.Close();
        }

        // 1
        public void loadDataSanPhamChon()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã chi tiết");
            dt.Columns.Add("Tên sản phẩm");
            dt.Columns.Add("Số lượng");
            dt.Columns.Add("Đơn giá");
            dt.Columns.Add("Thành tiền");

            // Reset data
            dgvSanPham.DataSource = dt;
            total = 0;

            foreach (ChiTietHoaDonMua ct in listChiTietMua)
            {
                SanPham sp = getSanPham(ct.product_id);
                dt.Rows.Add(ct.id, sp != null ? sp.name : "", ct.quantity, ct.price, ct.total);
                total += ct.total;
            }
            lbTien.Text = total.ToString();
            dgvSanPham.DataSource = dt;
        }
        //  Mã nhà cung cấp
        public void readDataSanPham(long suplier_id)
        {
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }

            String query = "SELECT * FROM tb_product WHERE suplier_id = @suplier_id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("suplier_id", suplier_id);
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


        public SanPham getSanPham(long product_id)
        {
            foreach (SanPham sp in listSanPham)
            {
                if (sp.id == product_id)
                {
                    return sp;
                }
            }
            return null;
        }


        public ChiTietHoaDonMua isExistChiTiet(long product_id)
        {
            foreach (ChiTietHoaDonMua ct in listChiTietMua)
            {
                if (ct.product_id == product_id)
                {
                    return ct;
                }
            }
            return null;
        }

       
        private void btnDatMon_Click(object sender, EventArgs e)
        {

            if (cbxNhaCungCap.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp");
                return;
            }
            if (cbxSanPham.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm");
                return;
            }

            // Lần đầu
            if (indexSuplierSelected == -1)
            {
                indexSuplierSelected = cbxNhaCungCap.SelectedIndex;
            }
            // Những lần sau
            else
            {
                if (indexSuplierSelected != cbxNhaCungCap.SelectedIndex)
                {
                    MessageBox.Show("Vui lòng chọn nhà cung cấp cùng loại");
                    cbxNhaCungCap.SelectedIndex = indexSuplierSelected;
                    return;
                }
            }
            SanPham sp = listSanPham[cbxSanPham.SelectedIndex];

            ChiTietHoaDonMua ct = new ChiTietHoaDonMua();
            ct.id = DateTime.Now.Ticks / 1000000;
            ct.price = sp.price_import;
            ct.quantity = int.Parse(dudSoLuong.Text);
            ct.total = ct.price * ct.quantity;
            ct.product_id = sp.id;



            ChiTietHoaDonMua tmp = isExistChiTiet(ct.product_id);

            if (tmp != null)
            {
                tmp.quantity += ct.quantity;
                tmp.total = tmp.quantity * tmp.price;

            }
            else
            {
                listChiTietMua.Add(ct);
            }
            loadDataSanPhamChon();
            dudSoLuong.SelectedIndex = 4;

        }


        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            long import_bill_id = writeDataHoaDonMua();
            if (import_bill_id == -1)
            {
                MessageBox.Show("Lỗi khi thêm hóa đơn");
                
            }
            // !writeDataChiTiet(import_bill_id)  => writeDataChiTiet(import_bill_id) == false
            // writeDataChiTiet(import_bill_id)  => writeDataChiTiet(import_bill_id) == true
            if (!writeDataChiTiet(import_bill_id)) // return true thì thực hiện
            {
                MessageBox.Show("Thanh toán thành công");
                return;
            }
            MessageBox.Show("Thanh toán thất bại");
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

        public bool writeDataChiTiet(long import_bill_id)
        {
            if (listChiTietMua.Count == 0)
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
            string query = "INSERT INTO tb_detail_import VALUES (@id, @product_id, @price, @quantity, @total, @import_id)";
            SqlCommand cmd = new SqlCommand(query, con);
            foreach (ChiTietHoaDonMua ct in listChiTietMua)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id", ct.id);
                cmd.Parameters.AddWithValue("product_id", ct.product_id);
                cmd.Parameters.AddWithValue("price", ct.price);
                cmd.Parameters.AddWithValue("quantity", ct.quantity);
                cmd.Parameters.AddWithValue("total", ct.total);
                cmd.Parameters.AddWithValue("import_id", import_bill_id);

                if (cmd.ExecuteNonQuery() <= 0)
                {
                    MessageBox.Show("Lỗi khi thêm chi tiết");
                    con.Close();
                    return false;
                }
            }
            


            foreach (ChiTietHoaDonMua ct in listChiTietMua)
            {
                SanPham sp = getSanPham(ct.product_id);

                if (!updateQuantity(ct.product_id, ct.quantity + sp.quantity))
                {
                    MessageBox.Show("Lỗi khi cập nhật số lượng");
                    return false;
                }
            }
            return true;
        }

        public long writeDataHoaDonMua()
        {
            HoaDonMua import_bill = new HoaDonMua();
            import_bill.id = DateTime.Now.Ticks / 1000000;
            import_bill.employee_id = 638539669000;
            import_bill.suplier_id = listNhaCungCap[indexSuplierSelected].id;
            import_bill.total = total;
            import_bill.date = DateTime.Now;

            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return -1;
            }
            string query = "INSERT INTO tb_import VALUES (@id, @total, @suplier_id, @employee_id, @date)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("id", import_bill.id);
            cmd.Parameters.AddWithValue("total", import_bill.total);
            cmd.Parameters.AddWithValue("suplier_id", import_bill.suplier_id);
            cmd.Parameters.AddWithValue("employee_id", import_bill.employee_id);
            cmd.Parameters.AddWithValue("date", import_bill.date);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Lỗi khi thêm hóa đơn");
                return -1;
            }
            
            return import_bill.id;

        }


        private void cbxNhaCungCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            long suplier_id = listNhaCungCap[cbxNhaCungCap.SelectedIndex].id;
            readDataSanPham(suplier_id);
            dudSoLuong.SelectedIndex = 4;
            
        }

        private void cbxSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            dudSoLuong.SelectedIndex = 4;
        }

       // 

        private void dgvSanPham_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            ChiTietChon = listChiTietMua[e.RowIndex];

            cbxNhaCungCap.SelectedIndex = indexSuplierSelected;

            string name_product = getSanPham(ChiTietChon.product_id).name;
            for (int i = 0; i < listSanPham.Count; i++)
            {
                if (listSanPham[i].name == name_product)
                {
                    cbxSanPham.SelectedIndex = i;
                    break;
                }
            }
            dudSoLuong.Text = ChiTietChon.quantity.ToString();
            btnDatMon.Enabled = false;
            cbxNhaCungCap.Enabled = false;
            cbxSanPham.Enabled = false;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (ChiTietChon == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa");
                return;
            }
            ChiTietChon.quantity = int.Parse(dudSoLuong.Text);
            ChiTietChon.total = ChiTietChon.quantity * ChiTietChon.price;

            btnDatMon.Enabled = true;
            cbxNhaCungCap.Enabled = true;
            cbxSanPham.Enabled = true;

            cbxSanPham.SelectedIndex = -1;

            loadDataSanPhamChon();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            listChiTietMua.Remove(ChiTietChon);
            ChiTietChon = null;
            btnDatMon.Enabled = true;
            cbxNhaCungCap.Enabled = true;
            cbxSanPham.Enabled = true;
            cbxSanPham.SelectedIndex = -1;
            loadDataSanPhamChon();
        }
    }
}
