using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataNCC
    {
        [DisplayName("Mã nhà cung cấp")]
        public int MaNCC { get; set; }
        [DisplayName("Tên nhà cung cấp")]
        public string TenNCC { get; set; }
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }
        [DisplayName("Số điện thoại")]
        public string DienThoai { get; set; }
    }
}