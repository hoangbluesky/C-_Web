using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;
using projectdbfirst.Models;

namespace projectdbfirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class NVsController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        public ActionResult Index(int? page, int? pSize)
        {
            ViewBag.Print = false;
            int ps = 5;
            if (pSize != null)
            {
                ps = db.NVs.Count();
                ViewBag.Print = true;
            }
            var ketQua = (from nv in db.NVs
                          from user in db.Users
                          where nv.NVUserId == user.UserId
                          select new NVUserViewModel
                          {
                              NVInfo = nv,
                              UserInfo = user
                          }).ToList();
            int pageSize = ps;
            int pageNumber = (page ?? 1);

            IPagedList<NVUserViewModel> pagedResult = ketQua.ToPagedList(pageNumber, pageSize);
            return View(pagedResult);
        }

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    NV nV = db.NVs.Find(id);
        //    if (nV == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(nV);
        //}

        public ActionResult Create(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNV,TenNV,GioiTinh,DiaChi,DienThoai,NgaySinh")] NV nV)
        {
            if (ModelState.IsValid)
            {
                db.NVs.Add(nV);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nV);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nvUserViewModel = GetNVUserViewModelById(id);
            if (nvUserViewModel == null)
            {
                return HttpNotFound();
            }
            return View(nvUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(projectdbfirst.Models.NVUserViewModel nvUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var nvToEdit = db.NVs.FirstOrDefault(c => c.NVUserId == nvUserViewModel.NVInfo.NVUserId);

                // Kiểm tra xem đối tượng KH có tồn tại hay không
                if (nvToEdit == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin cho đối tượng KH từ KHUserViewModel
                nvToEdit.TenNV = nvUserViewModel.NVInfo.TenNV;
                nvToEdit.GioiTinh = nvUserViewModel.NVInfo.GioiTinh;
                nvToEdit.DiaChi = nvUserViewModel.NVInfo.DiaChi;
                nvToEdit.DienThoai = nvUserViewModel.NVInfo.DienThoai;
                nvToEdit.NgaySinh = nvUserViewModel.NVInfo.NgaySinh;

                db.Entry(nvToEdit).State = EntityState.Modified;
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                //db.Entry(kH).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nvUserViewModel);
        }

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    NV nV = db.NVs.Find(id);
        //    if (nV == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(nV);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    NV nV = db.NVs.Find(id);
        //    db.NVs.Remove(nV);
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
        private NVUserViewModel GetNVUserViewModelById(int? id)
        {
            if (id == null)
            {
                return null;
            }

            // Lấy thông tin KH và User từ id
            var nv = db.NVs.Find(id);
            var user = db.Users.FirstOrDefault(u => u.UserId == nv.NVUserId);

            if (nv == null || user == null)
            {
                return null;
            }

            // Tạo một đối tượng KHUserViewModel và gán giá trị từ KH và User
            var nvUserViewModel = new NVUserViewModel
            {
                NVInfo = nv,
                UserInfo = user,
            };

            return nvUserViewModel;
        }
    }
}
