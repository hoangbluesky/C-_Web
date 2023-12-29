using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace projectdbfirst.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        Models.QLBHmvcEntities db = new Models.QLBHmvcEntities();
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Models.NV obj, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                //try
                //{
                //    Models.NV check = db.NVs.FirstOrDefault(m => m.TenTK == obj.TenTK && m.MatKhau == obj.MatKhau);
                //    if (check != null)
                //    {
                //        // login is successful
                //        //Session["User"] = check;
                //        // gi nhận đăng nhập với cookie
                //        FormsAuthentication.SetAuthCookie(check.TenTK, false);
                //        // UX: lấy returnUrl để điều hướng
                //        if (ReturnUrl == null || ReturnUrl == "")
                //        {
                //            return RedirectToAction("Index", "DanhMucs");
                //        }
                //        else
                //        {
                //            return Redirect(ReturnUrl);
                //        }

                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", "UserName or PassWord is wrong !");
                //    }
                //}
                //catch
                //{

                //}
            }
            return View(obj);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}