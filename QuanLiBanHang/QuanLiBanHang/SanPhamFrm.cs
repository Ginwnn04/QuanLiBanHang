using QuanLiBanHang.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace QuanLiBanHang
{
    public partial class SanPhamFrm : Form
    {
        List<SanPham> listSanPham = new List<SanPham>();
        List<NhaCungCap> listNhaCungCap = new List<NhaCungCap>();
        
        public SanPhamFrm()
        {
            InitializeComponent();
            readDataNhaCungCap();
            readData();
        }

        public void refresh()
        {
            txtMa.Text = "";
            txtTenSP.Text = "";
            txtGia.Text = "";
            txtGiaNhap.Text = "";
            cbxNhaCungCap.SelectedIndex = -1;
  
        }

        public void readData()
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
            }
            con.Close();
            loadData();
        }

        public void loadData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã");
            dt.Columns.Add("Tên sản phẩm");
            dt.Columns.Add("Số lượng tồn");
            dt.Columns.Add("Giá bán");
            dt.Columns.Add("Giá nhập");
            dt.Columns.Add("Nhà cung cấp");

            foreach (SanPham sp in listSanPham)
            {
                dt.Rows.Add(sp.id, sp.name, sp.quantity, sp.price, sp.price_import, getSuplierName(sp.suplier_id));
            }
            dgvSanPham.DataSource = dt;
        }

        public string getSuplierName(long suplier_id)
        {
            foreach (NhaCungCap ncc in listNhaCungCap)
            {
                if (ncc.id == suplier_id)
                {
                    return ncc.name;
                }
            }
            return "";
        }

        public void readDataNhaCungCap()
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


        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtTenSP.Text == "" || txtGia.Text == "" || txtGiaNhap.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }
            if (cbxNhaCungCap.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp");
                return;
            }

            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "INSERT INTO tb_product VALUES(@id, @name, @quantity, @price, @suplier_id, @price_import)";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("id", DateTime.Now.Ticks / 1000000);
            cmd.Parameters.AddWithValue("name", txtTenSP.Text);
            cmd.Parameters.AddWithValue("quantity", 0);
            cmd.Parameters.AddWithValue("suplier_id", listNhaCungCap[cbxNhaCungCap.SelectedIndex].id);
            try
            {
                long price = long.Parse(txtGia.Text);
                long price_import = long.Parse(txtGiaNhap.Text);
                if (price <= 0 || price_import <= 0)
                {
                    MessageBox.Show("Giá phải lớn hơn 0");
                    return;
                }
                else if (price_import > price)
                {
                    MessageBox.Show("Giá bán phải hơn giá nhập");
                    return;
                }
                cmd.Parameters.AddWithValue("price", price);
                cmd.Parameters.AddWithValue("price_import", price_import);
            }
            catch (FormatException fe)
            {
                MessageBox.Show("Giá phải là chữ số");
                return;
            }
          

            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Thêm sản phẩm thất bại");
                return;
            }
            MessageBox.Show("Thêm sản phẩm thành công");
            readData();
            refresh();
        }

        private void dgvSanPham_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= listSanPham.Count)
            {
                return;
            }
            SanPham sp = listSanPham[index];
            txtMa.Text = sp.id.ToString();
            txtTenSP.Text = sp.name;
            txtGia.Text = sp.price.ToString();    
            txtGiaNhap.Text = sp.price_import.ToString();

            for (int i = 0; i < listNhaCungCap.Count; i++)
            {
                if (listNhaCungCap[i].id == sp.suplier_id)
                {
                    cbxNhaCungCap.SelectedIndex = i;
                    break;
                }
            }

        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMa.Text == "")
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa");
                return;
            }
            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "DELETE FROM tb_product WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("id", txtMa.Text);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Xóa sản phẩm thất bại");
                return;
            }
            MessageBox.Show("Xóa sản phẩm thành công");
            readData();
            refresh();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtTenSP.Text == "" || txtGia.Text == "" || txtGiaNhap.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }
            if (cbxNhaCungCap.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm");
                return;
            }

            SqlConnection con = ConnectDB.getConnect();
            if (!ConnectDB.open())
            {
                MessageBox.Show("Kết nối thất bại");
                return;
            }
            String query = "UPDATE tb_product SET name = @name, price = @price, price_import = @price_import, suplier_id = @suplier_id WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("id", txtMa.Text);
            cmd.Parameters.AddWithValue("name", txtTenSP.Text);
            cmd.Parameters.AddWithValue("suplier_id", listNhaCungCap[cbxNhaCungCap.SelectedIndex].id);
            try
            {
                long price = long.Parse(txtGia.Text);
                long price_import = long.Parse(txtGiaNhap.Text);
                if (price <= 0 || price_import <= 0)
                {
                    MessageBox.Show("Giá phải lớn hơn 0");
                    return;
                }
                else if (price_import > price)
                {
                    MessageBox.Show("Giá nhập phải nhỏ hơn giá bán");
                    return;
                }
                cmd.Parameters.AddWithValue("price", price);
                cmd.Parameters.AddWithValue("price_import", price_import);
            }
            catch (FormatException fe)
            {
                MessageBox.Show("Giá phải là chữ số");
                return;
            }
      
            
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result <= 0)
            {
                MessageBox.Show("Sửa sản phẩm thất bại");
                return;
            }
            MessageBox.Show("Sửa sản phẩm thành công");
            readData();
            refresh();

        }
    }
}
