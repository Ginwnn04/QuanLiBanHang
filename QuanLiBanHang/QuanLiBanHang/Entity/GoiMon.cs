using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang.Entity
{
    public class GoiMon
    {
        public long id { get; set; }
        public int quantity { get; set; }   
        public long total { get; set; }
        public long employee_id { get; set; }
        public DateTime time_create { get; set; }
    }
}
