using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace projectdbfirst.Controllers
{
    public class AccountController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();
        // Action đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            if (TempData["SuccessMessageTaikhoan"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessageTaikhoan"].ToString();
            }
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(User userLogin, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                Models.User check = db.Users.FirstOrDefault(m => m.Username == userLogin.Username && m.Password == userLogin.Password);

                if (check != null)
                {
                    // Xác định vai trò của người dùng (admin hoặc client) từ cơ sở dữ liệu hoặc logic xác thực của bạn
                    string userRole = GetUserRole(userLogin.Username, userLogin.Password);

                    if (userRole == "admin")
                    {
                        FormsAuthentication.SetAuthCookie(check.Username, false);
                        // Gán quyền truy cập vào mảng roles nếu cần thiết
                        //string[] roles = { "admin" };
                        Session["AdminId"] = check.UserId;
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (userRole == "client")
                    {
                        FormsAuthentication.SetAuthCookie(check.Username, false);
                        // Gán quyền truy cập vào mảng roles nếu cần thiết
                        //string[] roles = { "client" };
                        Session["UserId"] = check.UserId;
                        //bool rememberMe = !string.IsNullOrEmpty(Request.Form["remember-me"]);
                        //if (rememberMe)
                        //{
                        //    HttpCookie cookie = new HttpCookie("RememberMe");
                        //    cookie.Value = "true";
                        //    cookie.Expires = DateTime.Now.AddDays(1); // Thời gian hết hạn của cookie
                        //    Response.Cookies.Add(cookie);
                        //}
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(userLogin);
        }


        // Hàm lấy vai trò của người dùng (ví dụ)
        private string GetUserRole(string username, string password)
        {
            var role = db.Users
                .Where(m => m.Username == username && m.Password == password)
                .Select(m => m.Role)
                .FirstOrDefault();
            var roleAsString = role?.ToString();
            return roleAsString;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(KHUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var check1 = db.Users.Any(m => m.Username == user.UserInfo.Username);
                var check2 = db.KHs.Any(m => m.Email == user.KHInfo.Email);
                if ( !check1 && !check2 )
                {
                    projectdbfirst.Models.User uta = new projectdbfirst.Models.User();
                    projectdbfirst.Models.KH khta = new projectdbfirst.Models.KH();

                    // user
                    uta.Username = user.UserInfo.Username;
                    uta.Password = user.UserInfo.Password;
                    uta.Role = "client";
                    db.Users.Add(uta);
                    db.SaveChanges();

                    // info kh is nearly null 

                    projectdbfirst.Models.User fi = db.Users.FirstOrDefault(m => m.Username == user.UserInfo.Username);
                    khta.Email = user.KHInfo.Email;
                    khta.FullName = "";
                    khta.DiaChi = "";
                    khta.DienThoai = "";
                    khta.KHUserId = fi.UserId;
                    db.KHs.Add(khta);
                    db.SaveChanges();
                    TempData["SuccessMessageTaikhoan"] = "Tạo tài khoản thành công!";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập, mật khẩu không đúng hoặc email đã tồn tại.");
                }
            }

            return View("Register");
        }

        [HttpGet]
        public ActionResult ForgotPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPass(string Email)
        {
            var check = db.KHs.FirstOrDefault(m => m.Email == Email);
            if (check == null)
                return View();
            else
            {
                // chưa biết
            }
            return View();
        }
    }
}