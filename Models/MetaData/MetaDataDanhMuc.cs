using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataDanhMuc
    {
        [DisplayName("Mã danh mục")]
        public int Id { get; set; }
        [DisplayName("Tên danh mục")]
        public string Ten { get; set; }

    }
}