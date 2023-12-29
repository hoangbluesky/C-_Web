using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataKH
    {
        [DisplayName("Mã khách hàng")]
        public int MaKH { get; set; }
        [DisplayName("Tên đầy đủ")]
        public string FullName { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$", ErrorMessage = "Please enter a valid phone number")]
        [DisplayName("Điện thoại")]
        public string DienThoai { get; set; }
        [DisplayName("khách hàng UserId")]
        public Nullable<int> KHUserId { get; set; }
    }
}