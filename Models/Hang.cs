//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace projectdbfirst.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hang()
        {
            this.CTHDBans = new HashSet<CTHDBan>();
            this.CTHDNhaps = new HashSet<CTHDNhap>();
        }
    
        public int MaH { get; set; }
        public string TenH { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public string MoTa { get; set; }
        public string Anh { get; set; }
        public Nullable<int> MaDanhMuc { get; set; }
        public Nullable<double> GiaTien { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTHDBan> CTHDBans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTHDNhap> CTHDNhaps { get; set; }
        public virtual DanhMuc DanhMuc { get; set; }
    }
}
