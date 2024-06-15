using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang.Entity
{
    public class SanPham
    {
        public SanPham() { }

        
       
        public long id { get; set; }
        public string name { get; set; }
        public long price { get; set; }
        public long price_import { get; set; }
        public int quantity { get; set; }

        public long suplier_id { get; set; }

    }
}
