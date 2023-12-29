using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataNV
    {
        [DisplayName("Mã nhân viên")]
        public int MaNV { get; set; }
        [DisplayName("Tên nhân viên")]
        public string TenNV { get; set; }
        [DisplayName("Giới tính")]
        public string GioiTinh { get; set; }
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }
        [DisplayName("Điện thoại")]
        public string DienThoai { get; set; }
        [DisplayName("Ngày sinh")]
        public Nullable<System.DateTime> NgaySinh { get; set; }
        [DisplayName("Nhân viên UserId")]
        public Nullable<int> NVUserId { get; set; }
    }
}