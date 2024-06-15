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
  
            

            int quantityNew = sp.quantity + chiTietChon.quantity;
            MessageBox.Show(sp.quantity + " " + chiTietChon.quantity + " " + quantityNew);
            lbSoLuong.Text = quantityNew.ToString();
            



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
            
            chiTietChon.quantity = int.Parse(dudSoLuong.Text);
            chiTietChon.total = chiTietChon.quantity * chiTietChon.price;
            lbSoLuong.Text =  (int.Parse(lbSoLuong.Text) - chiTietChon.quantity) + "";
            loadDataSanPhamChon();
            MessageBox.Show("Sửa thành công");
            btnDatMon.Enabled = true;
            cbxSanPham.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            cbxSanPham.SelectedIndex = -1;
            lbSoLuong.Text = "";
            dudSoLuong.Text = "1";
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            listChiTietDonBan.Remove(chiTietChon);
            lbSoLuong.Text = (int.Parse(lbSoLuong.Text) + chiTietChon.quantity) + "";
            loadDataSanPhamChon();
            MessageBox.Show("Xoá thành công");
            btnDatMon.Enabled = true;
            cbxSanPham.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            cbxSanPham.SelectedIndex = -1;
            lbSoLuong.Text = "";
            dudSoLuong.Text = "1";
        }
    }
    
}
