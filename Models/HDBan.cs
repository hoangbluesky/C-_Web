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
    
    public partial class HDBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HDBan()
        {
            this.CTHDBans = new HashSet<CTHDBan>();
        }
    
        public int MaHDBan { get; set; }
        public Nullable<System.DateTime> NgayBan { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<double> TongTien { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTHDBan> CTHDBans { get; set; }
        public virtual KH KH { get; set; }
    }
}
