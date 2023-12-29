using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projectdbfirst.Controllers
{
    [Authorize(Roles = "client")]
    public class WishlistController : Controller
    {
        Models.QLBHmvcEntities db = new Models.QLBHmvcEntities();
        // GET: Wishlist
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddToWishlist(int id)
        {
            List<Models.WishlistModel> lstWishlist = null;
            if (Session["Wishlist"] == null)
            {
                lstWishlist = new List<Models.WishlistModel>();
            }
            else
            {
                lstWishlist = (List<Models.WishlistModel>)Session["Wishlist"];
            }
            Models.WishlistModel obj = lstWishlist.FirstOrDefault(m => m.ProductID == id);
            if (obj == null)
            {
                Models.Hang crrProduct = db.Hangs.First(m => m.MaH == id);
                var sStatus = crrProduct.SoLuong > 0 ? "In Stock" : "Out of Stock";
                
                obj = new Models.WishlistModel
                {
                    ProductID = id,
                    ProductDetail = crrProduct,
                    StockStatus = sStatus,
                };
                lstWishlist.Add(obj);
            }
            else
            {
                return View("Index");
            }
            Session["Wishlist"] = lstWishlist;
            return View("Index");
        }
        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            if (Session["Wishlist"] != null)
            {
                List<Models.WishlistModel> WishlistItems = (List<Models.WishlistModel>)Session["Wishlist"];

                Models.WishlistModel productToRemove = WishlistItems.FirstOrDefault(item => item.ProductID == productId);

                if (productToRemove != null)
                {
                    WishlistItems.Remove(productToRemove);

                    Session["Wishlist"] = WishlistItems;
                }
            }

            return RedirectToAction("Index");
        }
    }
}