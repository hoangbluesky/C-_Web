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
    public class CTHDBansController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        // GET: Admin/CTHDBans
        public ActionResult Index(int? page ,int? id)
        {
            var cTHDBans = db.CTHDBans.Include(c => c.Hang).Include(c => c.HDBan).ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pageResult = cTHDBans.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }

        // GET: Admin/CTHDBans/Details/5
        public ActionResult Details(int? key1, int? key2)
        {
            if (key1 == null || key2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTHDBan cTHDBan = db.CTHDBans.Find(key1, key2);
            if (cTHDBan == null)
            {
                return HttpNotFound();
            }
            return View(cTHDBan);
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
