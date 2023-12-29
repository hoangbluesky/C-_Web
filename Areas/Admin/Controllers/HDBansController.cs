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
using System.Drawing.Printing;

namespace projectdbfirst.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class HDBansController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        // GET: Admin/HDBans
        public ActionResult Index(int? page)
        {
            var lst = db.HDBans.Include(h => h.KH).ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pageResult = lst.ToPagedList(pageNumber, pageSize);

            return View(pageResult);
        }

        // GET: Admin/HDBans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HDBan hDBan = db.HDBans.Find(id);
            if (hDBan == null)
            {
                return HttpNotFound();
            }
            return View(hDBan);
        }

        // GET: Admin/HDBans/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    HDBan hDBan = db.HDBans.Find(id);
        //    if (hDBan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MaKH = new SelectList(db.KHs, "MaKH", "FullName", hDBan.MaKH);
        //    return View(hDBan);
        //}

        // POST: Admin/HDBans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "MaHDBan,NgayBan,MaKH,TongTien")] HDBan hDBan)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(hDBan).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MaKH = new SelectList(db.KHs, "MaKH", "FullName", hDBan.MaKH);
        //    return View(hDBan);
        //}

        // GET: Admin/HDBans/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    HDBan hDBan = db.HDBans.Find(id);
        //    if (hDBan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(hDBan);
        //}

        // POST: Admin/HDBans/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    HDBan hDBan = db.HDBans.Find(id);
        //    db.HDBans.Remove(hDBan);
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
    }
}
