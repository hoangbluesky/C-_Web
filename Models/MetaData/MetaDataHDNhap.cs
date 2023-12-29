using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.Metadata
{
    public class MetaDataHDNhap
    {
        [DisplayName("Mã hóa đơn nhập")]
        public int MaHDNhap { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Ngày nhập")]
        public Nullable<System.DateTime> NgayNhap { get; set; }
        [DisplayName("Mã nhà cung cấp")]
        public Nullable<int> MaNCC { get; set; }
        [DisplayName("Tổng tiền")]
        public Nullable<double> TongTien { get; set; }
        [DisplayName("Mã Nhân viên")]
        public Nullable<int> MaNV { get; set; }
    }
}