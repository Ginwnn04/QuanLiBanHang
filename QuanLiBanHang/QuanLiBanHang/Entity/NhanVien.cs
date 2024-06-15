using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiBanHang.Entity
{
    public class NhanVien
    {
        public long id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string city { get; set; }

        public long role_id { get; set; }

        public int gender { get; set; }

        public DateTime date_of_birth { get; set; }
    }
}
