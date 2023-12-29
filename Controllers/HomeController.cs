using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;

namespace projectdbfirst.Controllers
{
    [Authorize(Roles = "client")]
    public class HomeController : Controller
    {
    Models.QLBHmvcEntities db = new Models.QLBHmvcEntities();
        // GET: home
        public ActionResult Index()
        {
            List<Models.Hang> data = db.Hangs.Take(10).ToList();
            ViewBag.Data = data;
            return View();
        }

        public ActionResult Details(int? id)
        {
            List<Models.Hang> data = db.Hangs.Take(5).ToList();
            ViewBag.Data = data;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Hang hang = db.Hangs.Find(id);
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
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

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Shop(int? page)
        {
            var lst = db.Hangs.ToList();
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            IPagedList<Hang> pageResult = lst.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }
        [HttpPost]
        public ActionResult Shop(string search, int? page)
        {
            var lst = db.Hangs.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                lst = lst.Where(m => m.TenH.Contains(search)).ToList();
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            IPagedList<Hang> pageResult = lst.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }
    }
}