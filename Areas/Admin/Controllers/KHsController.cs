using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using projectdbfirst.Models;
using PagedList;
using System.Drawing.Printing;

namespace projectdbfirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class KHsController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        public ActionResult Index(int? page, int? pSize)
        {
            ViewBag.Print = false;
            int ps = 5;
            if (pSize != null)
            {
                ps = db.KHs.Count();
                ViewBag.Print = true;
            }
            var ketQua = (from kh in db.KHs
                          from user in db.Users
                          where kh.KHUserId == user.UserId
                          select new KHUserViewModel
                          {
                              KHInfo = kh,
                              UserInfo = user
                          }).ToList();

            int pageSize = ps; 
            int pageNumber = (page ?? 1); 

            IPagedList<KHUserViewModel> pagedResult = ketQua.ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }

        public ActionResult Create(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KH kH)
        {
            if (ModelState.IsValid)
            {
                db.KHs.Add(kH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kH);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var khUserViewModel = GetKHUserViewModelById(id);
            if (khUserViewModel == null)
            {
                return HttpNotFound();
            }
            return View(khUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(projectdbfirst.Models.KHUserViewModel khUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var khToEdit = db.KHs.FirstOrDefault(c => c.KHUserId == khUserViewModel.KHInfo.KHUserId);

                // Kiểm tra xem đối tượng KH có tồn tại hay không
                if (khToEdit == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin cho đối tượng KH từ KHUserViewModel
                khToEdit.FullName = khUserViewModel.KHInfo.FullName;
                khToEdit.DiaChi = khUserViewModel.KHInfo.DiaChi;
                khToEdit.DienThoai = khUserViewModel.KHInfo.DienThoai;
                khToEdit.Email = khUserViewModel.KHInfo.Email;

                db.Entry(khToEdit).State = EntityState.Modified;
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                //db.Entry(kH).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khUserViewModel);
        }

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    KH kH = db.KHs.Find(id);
        //    if (kH == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(kH);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    KH kH = db.KHs.Find(id);
        //    db.KHs.Remove(kH);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private KHUserViewModel GetKHUserViewModelById(int? id)
        {
            if (id == null)
            {
                return null;
            }

            // Lấy thông tin KH và User từ id
            var kh = db.KHs.Find(id);
            var user = db.Users.FirstOrDefault(u => u.UserId == kh.KHUserId);

            if (kh == null || user == null)
            {
                return null;
            }

            // Tạo một đối tượng KHUserViewModel và gán giá trị từ KH và User
            var khUserViewModel = new KHUserViewModel
            {
                KHInfo = kh,
                UserInfo = user,
            };

            return khUserViewModel;
        }
    }
}
