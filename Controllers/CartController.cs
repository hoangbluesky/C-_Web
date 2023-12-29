using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projectdbfirst.Controllers
{
    [Authorize(Roles = "client")]
    public class CartController : Controller
    {
        Models.QLBHmvcEntities db = new Models.QLBHmvcEntities();
        // GET: Cart
        public ActionResult Index()
        {
            if (TempData["SuccessMessageBuy"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessageBuy"].ToString();
            }
            return View();
        }

        public ActionResult AddToCart(int id)
        {
            List<Models.CartModel> lstCart = null;
            if (Session["Cart"] == null)
            {
                lstCart = new List<Models.CartModel>();
            }
            else
            {
                lstCart = (List<Models.CartModel>)Session["Cart"];
            }
            Models.CartModel obj = lstCart.FirstOrDefault(m => m.ProductID == id);
            if (obj == null)
            {
                Models.Hang crrProduct = db.Hangs.First(m => m.MaH == id);
                obj = new Models.CartModel
                {
                    ProductID = id,
                    ProductDetail = crrProduct,
                    Quantity = 1,
                    Amount = (double)(1 * crrProduct.GiaTien),
                    Note = "",
                };
                lstCart.Add(obj);
            }
            else
            {
                obj.Quantity = obj.Quantity + 1;
                obj.Amount = (double)(obj.Quantity * obj.ProductDetail.GiaTien);
            }
            Session["Cart"] = lstCart;
            return View("Index");
        }

        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            // Kiểm tra xem Session "Cart" có tồn tại không
            if (Session["Cart"] != null)
            {
                List<Models.CartModel> cartItems = (List<Models.CartModel>)Session["Cart"];

                // Tìm sản phẩm trong giỏ hàng với productId cần xóa
                Models.CartModel productToRemove = cartItems.FirstOrDefault(item => item.ProductID == productId);

                if (productToRemove != null)
                {
                    // Xóa sản phẩm khỏi giỏ hàng
                    cartItems.Remove(productToRemove);

                    // Cập nhật Session "Cart" sau khi xóa
                    Session["Cart"] = cartItems;
                }
            }

            // Sau khi xóa, có thể chuyển hướng người dùng đến trang giỏ hàng hoặc trang tương tự
            return RedirectToAction("Index");
        }

        public ActionResult BuyProduct()
        {
            if (Session["Cart"] != null)
            {
                List<Models.CartModel> lst = (List<Models.CartModel>)Session["Cart"];

                // create HDBan
                projectdbfirst.Models.HDBan hdBan = new projectdbfirst.Models.HDBan();
                double tongtien = 0;
                // get current date
                //DateTime currentDate = DateTime.Now;
                //string formattedDate = currentDate.ToString("MM/dd/yyyy");
                // get MaKH
                int id = (int)Session["UserId"];
                //int id = 2;
                var kh = db.KHs.FirstOrDefault(m => m.KHUserId == id);
                hdBan.NgayBan = DateTime.Now;
                hdBan.MaKH = kh.MaKH;
                hdBan.TongTien = tongtien;

                db.HDBans.Add(hdBan);
                db.SaveChanges();

                // create all CTHDBan
                
                List<projectdbfirst.Models.CTHDBan> cthdBanList = new List<projectdbfirst.Models.CTHDBan>();
                // get MaHDBan
                int newMaHDBan = hdBan.MaHDBan;
                for( int i = 0; i < lst.Count; i++ )
                {
                    projectdbfirst.Models.CTHDBan ctHDBan = new projectdbfirst.Models.CTHDBan();
                    ctHDBan.MaHDBan = newMaHDBan;
                    ctHDBan.MaH = lst[i].ProductDetail.MaH;
                    ctHDBan.SoLuong = lst[i].Quantity;
                    ctHDBan.DonGia = lst[i].ProductDetail.GiaTien;
                    ctHDBan.ThanhTien = lst[i].Amount;

                    tongtien += lst[i].Amount;

                    cthdBanList.Add(ctHDBan);
                }
                // update SoLuong in Hang
                foreach (var ctHDBan in cthdBanList)
                {
                    var hangToUpdate = db.Hangs.FirstOrDefault(c => c.MaH == ctHDBan.MaH);
                    if (hangToUpdate != null)
                    {
                        hangToUpdate.SoLuong -= ctHDBan.SoLuong;
                    }
                }
                db.SaveChanges();

                db.CTHDBans.AddRange(cthdBanList);
                db.SaveChanges();

                // update tongtien in HDBan
                hdBan.TongTien = tongtien;
                db.SaveChanges();

                Session["Cart"] = null;
                TempData["SuccessMessageBuy"] = "Thao tác đã hoàn thành thành công!";
            }
            return RedirectToAction("Index");
        }
    }
}