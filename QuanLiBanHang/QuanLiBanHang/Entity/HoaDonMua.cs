using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang.Entity
{
    public class HoaDonMua
    {
        public long id { get; set; }
        public long employee_id { get; set; }
        public long suplier_id { get; set; }
        public DateTime date { get; set; }
        public long total { get; set; }
  
    }
}
