using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projectdbfirst.Controllers
{
    [Authorize(Roles = "client")]
    public class MyAccoutController : Controller
    {
        QLBHmvcEntities db = new QLBHmvcEntities();
        // GET: MyAccout
        public ActionResult Index()
        {
            int id = (int)Session["UserId"];
            //int id = 2;
            var kh = db.KHs.FirstOrDefault(k => k.KHUserId == id);
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (kh == null || user == null)
            {
                return HttpNotFound(); // Xử lý khi không tìm thấy đối tượng KH hoặc User
            }

            var viewModel = new UserDetailsViewModel
            {
                Kh = kh,
                User = user
            };

            List<projectdbfirst.Models.HDBan> dsHDBan = db.HDBans.Where(m => m.MaKH == kh.MaKH).ToList();
            ViewBag.DanhSachHDBan = dsHDBan;

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult UpdateUser(UserDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var utud = db.Users.FirstOrDefault(u => u.UserId == viewModel.User.UserId);

                if (utud != null)
                {
                    // Cập nhật thông tin từ ViewModel vào đối tượng KH và User tương ứng
                    utud.Username = viewModel.User.Username; // Ví dụ cập nhật thuộc tính Username
                    utud.Password = viewModel.User.Password;

                    // Cập nhật vào cơ sở dữ liệu
                    db.Users.Attach(utud);
                    db.Entry(utud).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index"); // Hoặc chuyển hướng đến trang khác sau khi cập nhật thành công
                }
            }

            return View("Index",viewModel); // Trả về View nếu có lỗi xảy ra hoặc dữ liệu không hợp lệ
        }

        [HttpPost]
        public ActionResult UpdateKH(UserDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var khtud = db.KHs.FirstOrDefault(k => k.KHUserId == viewModel.Kh.KHUserId);

                if (khtud != null)
                {
                    // Cập nhật thông tin từ ViewModel vào đối tượng KH và User tương ứng
                    khtud.FullName = viewModel.Kh.FullName;
                    khtud.DiaChi = viewModel.Kh.DiaChi;
                    khtud.DienThoai = viewModel.Kh.DienThoai;
                    khtud.Email = viewModel.Kh.Email;

                    // Cập nhật vào cơ sở dữ liệu
                    db.KHs.Attach(khtud);
                    db.Entry(khtud).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index"); // Hoặc chuyển hướng đến trang khác sau khi cập nhật thành công
                }
            }

            return View("Index",viewModel); // Trả về View nếu có lỗi xảy ra hoặc dữ liệu không hợp lệ
        }

        public ActionResult DetailsCTHDBan(int id)
        {
            var All = db.CTHDBans.Include(c => c.Hang).Include(c => c.HDBan);
            var cTHDBans = All.ToList();
            if (id != 0)
            {
                cTHDBans = cTHDBans.Where(c => c.MaHDBan == id).ToList();
            }
            return View(cTHDBans.ToList());
        }
    }
}