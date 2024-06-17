using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang.Entity
{
    public class ChiTietHoaDonMua
    {
        public long id { get; set; }
        public long product_id { get; set; }
        public long import_id { get; set; }
        public long price { get; set; }
        public int quantity { get; set; }
        public long total { get; set; }
    }
}
