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

namespace projectdbfirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class HDNhapsController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        public ActionResult Index(int? page)
        {
            if (TempData["SuccessMessageHDNhap"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessageHDNhap"].ToString();
            }
            if (TempData["ErrorMessageHDNhap"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessageHDNhap"].ToString();
            }
            var lst = db.HDNhaps.Include(h => h.NCC).Include(h => h.NV).ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pageResult = lst.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HDNhap hDNhap = db.HDNhaps.Find(id);
            if (hDNhap == null)
            {
                return HttpNotFound();
            }
            return View(hDNhap);
        }

        public ActionResult Create()
        {
            ViewBag.MaNCC = new SelectList(db.NCCs, "MaNCC", "TenNCC");
            ViewBag.MaNV = new SelectList(db.NVs, "MaNV", "TenNV");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHDNhap,NgayNhap,MaNCC,TongTien,MaNV")] HDNhap hDNhap)
        {
            if (ModelState.IsValid)
            {
                hDNhap.TongTien = 0;
                db.HDNhaps.Add(hDNhap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNCC = new SelectList(db.NCCs, "MaNCC", "TenNCC", hDNhap.MaNCC);
            ViewBag.MaNV = new SelectList(db.NVs, "MaNV", "TenNV", hDNhap.MaNV);
            return View(hDNhap);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HDNhap hDNhap = db.HDNhaps.Find(id);
            if (hDNhap == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNCC = new SelectList(db.NCCs, "MaNCC", "TenNCC", hDNhap.MaNCC);
            ViewBag.MaNV = new SelectList(db.NVs, "MaNV", "TenNV", hDNhap.MaNV);
            return View(hDNhap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHDNhap,NgayNhap,MaNCC,TongTien,MaNV")] HDNhap hDNhap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hDNhap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNCC = new SelectList(db.NCCs, "MaNCC", "TenNCC", hDNhap.MaNCC);
            ViewBag.MaNV = new SelectList(db.NVs, "MaNV", "TenNV", hDNhap.MaNV);
            return View(hDNhap);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HDNhap hDNhap = db.HDNhaps.Find(id);
            if (hDNhap == null)
            {
                return HttpNotFound();
            }
            return View(hDNhap);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var hDNhap = db.HDNhaps.Find(id);
            if( hDNhap == null)
            {
                return HttpNotFound();
            }
            var check = db.CTHDNhaps.Any( c => c.MaHDNhap == id);
            if( check )
            {
                TempData["ErrorMessageHDNhap"] = "Mã hóa đơn hiện tại đang được sử dụng! không thê xóa!";
            }
            else
            {
                db.HDNhaps.Remove(hDNhap);
                db.SaveChanges();
                TempData["SuccessMessageHDNhap"] = "Thao tác đã hoàn thành thành công!";
            }
           
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
