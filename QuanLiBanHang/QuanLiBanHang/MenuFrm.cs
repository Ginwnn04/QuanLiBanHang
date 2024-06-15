using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiBanHang
{
    public partial class MenuFrm : Form
    {
        public MenuFrm()
        {
            InitializeComponent();
            // Align the form to the center of the screen
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private Form current;
        private void openChildForm(Form childForm)
        {
            if (current != null)
            {
                current.Close();
            }
            current = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnBody.Controls.Add(childForm);
            pnBody.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

      

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            openChildForm(new NhanVienFrm());
        }

        private void btnNhaCungCap_Click(object sender, EventArgs e)
        {
            openChildForm(new NhaCungCapFrm());
        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            openChildForm(new KhachHangFrm());
        }

        private void btnChucVu_Click(object sender, EventArgs e)
        {
            openChildForm(new ChucVuFrm());
        }

        private void btnHangHoa_Click(object sender, EventArgs e)
        {
            openChildForm(new SanPhamFrm());
        }

        private void btnMuaHang_Click(object sender, EventArgs e)
        {
            openChildForm(new MuaHangFrm());
        }
    }
}
