using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataHang
    {
        public int MaH { get; set; }
        [DisplayName("Tên hàng")]
        public string TenH { get; set; }
        [DisplayName("Số lượng")]
        public Nullable<int> SoLuong { get; set; }
        [DisplayName("Mô tả")]
        public string MoTa { get; set; }
        [DisplayName("Ảnh sản phẩm")]
        public string Anh { get; set; }
        [DisplayName("Mã danh mục")]
        public Nullable<int> MaDanhMuc { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency)]
        [DisplayName("Giá tiền")]
        public Nullable<double> GiaTien { get; set; }
        [DisplayName("Giảm giá")]
        [Range(0, 100, ErrorMessage = "Giảm giá phải nằm trong khoảng từ 0 đến 50")]
        public Nullable<int> GiamGia { get; set; }
    }
}