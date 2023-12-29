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
    public class CTHDNhapsController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        public ActionResult Index(int? page, int? Id)
        {
            var All = db.CTHDNhaps.Include(c => c.Hang).Include(c => c.HDNhap);
            var cTHDNhaps = All.ToList();
            if( Id != 0)
            {
                cTHDNhaps = cTHDNhaps.Where(c => c.MaHDNhap == Id).ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pageResult = cTHDNhaps.ToPagedList(pageNumber, pageSize);
            ViewBag.ID = Id;
            return View(pageResult);
        }

        public ActionResult Details(int? key1, int? key2)
        {
            if (key1 == null || key2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTHDNhap cTHDNhap = db.CTHDNhaps.Find(key1, key2);
            if (cTHDNhap == null)
            {
                return HttpNotFound();
            }
            return View(cTHDNhap);
        }

        public ActionResult Create(int? Id)
        {
            ViewBag.MaH = new SelectList(db.Hangs, "MaH", "TenH");
            ViewBag.MaDanhMucHienTai = Id;
            var selected = db.HDNhaps.ToList();
            if (Id != 0)
            {
                var Ma = db.HDNhaps.Where(c => c.MaHDNhap == Id);
                selected = Ma.ToList();
            }
            ViewBag.MaHDNhap = new SelectList(selected, "MaHDNhap", "MaHDNhap");

            var selectedHang = db.Hangs.FirstOrDefault();
            ViewBag.GiaTien = (selectedHang != null) ? selectedHang.GiaTien : 0; 

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHDNhap,MaH,SoLuong,DonGia,ThanhTien")] CTHDNhap cTHDNhap)
        {
            if (ModelState.IsValid)
            {
                var check = db.CTHDNhaps.Any(c => c.MaH == cTHDNhap.MaH && c.MaHDNhap == cTHDNhap.MaHDNhap);
                if ( !check )
                {
                    // Thêm mới vào bảng CTHDNhap
                    db.CTHDNhaps.Add(cTHDNhap);
                    db.SaveChanges();

                    // Cập nhật số lượng trong bảng Hangs
                    var hangToUpdate = db.Hangs.FirstOrDefault(c => c.MaH == cTHDNhap.MaH);
                    if( hangToUpdate != null)
                    {
                        hangToUpdate.SoLuong += cTHDNhap.SoLuong;
                    }

                    // cập nhập tông tiền trong bảng hóa đơn
                    var hd = db.HDNhaps.FirstOrDefault(m => m.MaHDNhap == cTHDNhap.MaHDNhap);
                    hd.TongTien += cTHDNhap.ThanhTien;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = cTHDNhap.MaHDNhap });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Mặt hàng đã tồn tại trong CTHDNhap này.");
                }
            }

            ViewBag.MaH = new SelectList(db.Hangs, "MaH", "TenH", cTHDNhap.MaH);
            ViewBag.MaHDNhap = new SelectList(db.HDNhaps, "MaHDNhap", "MaHDNhap", cTHDNhap.MaHDNhap);
            return View(cTHDNhap);
        }

        public ActionResult Edit(int? key1, int? key2)
        {
            if (key1 == null || key2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cTHDNhap = db.CTHDNhaps.Find(key1, key2);
            if (cTHDNhap == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaH = new SelectList(db.Hangs, "MaH", "TenH", cTHDNhap.MaH);
            ViewBag.MaHDNhap = new SelectList(db.HDNhaps, "MaHDNhap", "MaHDNhap", cTHDNhap.MaHDNhap);
            return View(cTHDNhap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHDNhap,MaH,SoLuong,DonGia,ThanhTien")] CTHDNhap cTHDNhap)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin về số lượng trước khi chỉnh sửa
                var oldCTHDNhap = db.CTHDNhaps.AsNoTracking().FirstOrDefault(c => c.MaHDNhap == cTHDNhap.MaHDNhap && c.MaH == cTHDNhap.MaH);
                int oldSoLuong = (int)oldCTHDNhap.SoLuong;
                float oldThanhTien = (float)oldCTHDNhap.ThanhTien;

                db.Entry(cTHDNhap).State = EntityState.Modified;
                db.SaveChanges();

                // Tính toán sự thay đổi về số lượng
                int newSoLuong = (int)cTHDNhap.SoLuong;
                int changeInQuantity = newSoLuong - oldSoLuong;

                // Tính toán sự thay đổi về thành tiền
                float newThanhTien = (float)cTHDNhap.ThanhTien;
                float changeInThanhTien = newThanhTien - oldThanhTien;

                // Cập nhật số lượng trong bảng Hangs
                var hangToUpdate = db.Hangs.FirstOrDefault(h => h.MaH == cTHDNhap.MaH);
                if (hangToUpdate != null)
                {
                    hangToUpdate.SoLuong += changeInQuantity;
                }

                // cập nhập tổng tiền 
                var hd = db.HDNhaps.FirstOrDefault(m => m.MaHDNhap == cTHDNhap.MaHDNhap);
                hd.TongTien += changeInThanhTien;
                db.SaveChanges();

                return RedirectToAction("Index", new { id = cTHDNhap.MaHDNhap });
            }
            ViewBag.MaH = new SelectList(db.Hangs, "MaH", "TenH", cTHDNhap.MaH);
            ViewBag.MaHDNhap = new SelectList(db.HDNhaps, "MaHDNhap", "MaHDNhap", cTHDNhap.MaHDNhap);
            return View(cTHDNhap);
        }

        public ActionResult Delete(int? key1, int? key2)
        {
            if (key1 == null || key2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTHDNhap cTHDNhap = db.CTHDNhaps.Find(key1, key2);
            if (cTHDNhap == null)
            {
                return HttpNotFound();
            }
            return View(cTHDNhap);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int key1, int key2)
        {
            CTHDNhap cTHDNhap = db.CTHDNhaps.Find(key1, key2);
            db.CTHDNhaps.Remove(cTHDNhap);
            db.SaveChanges();
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

        [HttpGet]
        public ActionResult GetPrice(int id)
        {
            var selectedHang = db.Hangs.FirstOrDefault(h => h.MaH == id);
            var price = (selectedHang != null) ? selectedHang.GiaTien : 0; // Giả sử GiaTien là trường chứa giá tiền của mã hàng
            return Content(price.ToString()); // Trả về giá tiền dưới dạng text
        }
    }
}
