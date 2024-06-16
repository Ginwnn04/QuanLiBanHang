using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang.Entity
{
    public class HoaDonBan
    {
        public long id { get; set; }
        public long customer_id { get; set; }
        public long employee_id { get; set; }
        public DateTime date { get; set; }
        public long total { get; set; }
    }
}
