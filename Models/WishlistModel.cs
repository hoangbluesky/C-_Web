using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models
{
    public class WishlistModel
    {
        public int ProductID { get; set; }
        public Models.Hang ProductDetail { get; set; }
        public string StockStatus { get; set; }
    }
}