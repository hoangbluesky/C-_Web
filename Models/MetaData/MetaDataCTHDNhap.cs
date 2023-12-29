using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataCTHDNhap
    {
        [DisplayName("Mã hóa đơn nhập")]
        public int MaHDNhap { get; set; }
        [DisplayName("Mã hàng")]
        public int MaH { get; set; }
        [DisplayName("Số lượng")]
        public Nullable<int> SoLuong { get; set; }
        [DisplayName("Đơn giá")]
        public Nullable<double> DonGia { get; set; }
        [DisplayName("Thành tiền")]
        public Nullable<double> ThanhTien { get; set; }

    }
}