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
    public class UsersController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        // GET: Admin/Users
        public ActionResult Index(int? page)
        {
            var lst = db.Users.ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            IPagedList<projectdbfirst.Models.User> pageResult = lst.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }

        // GET: Admin/Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            var roles = new List<string> { "client", "admin" };
            ViewBag.Role = new SelectList(roles);
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
            {
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(m => m.Username == user.Username);
                if( check == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    if (user.Role == "client")
                    {
                        return RedirectToAction("Create", "KHs", new { id = user.UserId, area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Create", "NVs", new { id = user.UserId, area = "Admin" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "UserName is available !");
                }
            }

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var roles = new List<string> { "client", "admin" };
            ViewBag.Role = new SelectList(roles, user.Role);
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Username,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            if( user.Role == "client" )
            {
                projectdbfirst.Models.KH kh = db.KHs.FirstOrDefault(m => m.KHUserId == user.UserId);
                db.KHs.Remove(kh);
            }
            else
            {
                projectdbfirst.Models.NV nv = db.NVs.FirstOrDefault(m => m.NVUserId == user.UserId);
                db.NVs.Remove(nv);
            }
            db.Users.Remove(user);
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
    }
}
