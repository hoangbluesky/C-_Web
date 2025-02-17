﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using projectdbfirst.Models;
using PagedList;
using System.Text.RegularExpressions;
using static projectdbfirst.Controllers.HomeController;

namespace projectdbfirst.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    public class HangsController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();

        public ActionResult Index(int? page, int? Id)
        {
            if (TempData["SuccessMessageHang"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessageHang"].ToString();
            }
            if (TempData["ErrorMessageHang"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessageHang"].ToString();
            }

            var All = db.Hangs.Include(h => h.DanhMuc);
            var hangs = All.ToList();
            
            if ( Id != 0 )
            {
                hangs = hangs.Where(h => h.MaDanhMuc == Id).ToList(); 
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            IPagedList<Hang> pageHang = hangs.ToPagedList(pageNumber, pageSize);
            ViewBag.DanhMucHienTai = Id;
            return View(pageHang);
        }
        [HttpPost]
        public ActionResult Index(string search, int? page, int? Id)
        {
            var All = db.Hangs.Include(h => h.DanhMuc);
            var hangs = All.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                search = RemoveSpecialCharacters(search);

                // Lọc danh sách theo chuỗi search đã được làm sạch
                hangs = hangs.Where(m => m.TenH.Contains(search)).ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            IPagedList<Hang> pageHang = hangs.ToPagedList(pageNumber, pageSize);
            ViewBag.DanhMucHienTai = Id;
            return View(pageHang);
        }

        public string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Loại bỏ ký tự đặc biệt (chỉ giữ lại chữ cái, chữ số và khoảng trắng)
            string pattern = @"[^a-zA-Z0-9\s]";
            return Regex.Replace(input, pattern, string.Empty);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hang hang = db.Hangs.Find(id);
            if (hang == null)
            {
                return HttpNotFound();
            }

            var chats = db.Chats.Where(c => c.MaH == id).ToList();
            var chatTree = BuildChatTree(chats);

            ViewBag.ChatTree = chatTree;

            return View(hang);
        }

        public ActionResult Create(int ?id)
        {
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "Id", "Ten", id);
            ViewBag.MaDanhMucHienTai = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Hang hang)
        {
            if (ModelState.IsValid)
            {
                var f = Request.Files["fAvatar"];
                if (f != null && f.ContentLength > 0)
                {
                    string folderName = Server.MapPath("~/Assets/Uploads");
                    string fileName = f.FileName;
                    f.SaveAs(folderName + "/" + fileName);
                    hang.Anh = "/Assets/Uploads/" + fileName;
                }
                db.Hangs.Add(hang);
                db.SaveChanges();
                return RedirectToAction("Index", new { page = 1, id = hang.MaDanhMuc });
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "Id", "Ten", hang.MaDanhMuc);
            return View(hang);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hang hang = db.Hangs.Find(id);
            if (hang == null)
            {
                return HttpNotFound();
            }
            string anhs = db.Hangs.Where(m => m.MaH == id).Select(c => c.Anh).FirstOrDefault();
            ViewBag.Anhs = anhs;
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "Id", "Ten", hang.MaDanhMuc);
            return View(hang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Hang hang)
        {
            if (ModelState.IsValid)
            {
                var f = Request.Files["fAvatar"];
                if (f != null && f.ContentLength > 0)
                {
                    string folderName = Server.MapPath("~/Assets/Uploads");
                    string fileName = f.FileName;
                    f.SaveAs(folderName + "/" + fileName);
                    hang.Anh = "/Assets/Uploads/" + fileName;
                }
                else
                {
                    var existingHang = db.Hangs.AsNoTracking().FirstOrDefault(m => m.MaH == hang.MaH);
                    hang.Anh = existingHang.Anh;
                }
                db.Entry(hang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = hang.MaDanhMuc });
            }
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "Id", "Ten", hang.MaDanhMuc);
            return View(hang);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hang hang = db.Hangs.Find(id);
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? key)
        {
            var hang = db.Hangs.Find(id);
            if( hang == null)
            {
                return HttpNotFound();
            }
            var check = db.CTHDNhaps.Any(c => c.MaH == id);
            if( check )
            {
                TempData["ErrorMessageHang"] = "sản phẩm này hiện đang sử dụng ! không thể xóa!";
            }
            else
            {
                db.Hangs.Remove(hang);
                db.SaveChanges();
                TempData["SuccessMessageHang"] = "Thao tác đã hoàn thành thành công!";
            }
            //Hang hang = db.Hangs.Find(id);
            //db.Hangs.Remove(hang);
            //db.SaveChanges();
            return RedirectToAction("Index", new { Id = key });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private List<ChatNode> BuildChatTree(List<Chat> chats)
        {
            var chatDict = new Dictionary<int, ChatNode>();
            var rootChats = new List<ChatNode>();

            // Tạo các đối tượng ChatNode và thêm vào từ điển
            foreach (var chat in chats)
            {
                var kh = db.KHs.FirstOrDefault(k => k.MaKH == chat.MaKH);
                var node = new ChatNode
                {
                    ChatId = chat.ChatId,
                    TenKH = kh?.FullName,
                    MaH = chat.MaH,
                    ParentChatId = chat.ParentChatId,
                    Content = chat.Content,
                    Time = chat.Time,
                    Children = new List<ChatNode>()
                };
                chatDict[chat.ChatId] = node;

                // Nếu ParentChatId là -1, thêm vào danh sách gốc
                if (chat.ParentChatId == -1)
                {
                    rootChats.Add(node);
                }
            }

            // Gắn các node con vào node cha
            foreach (var chat in chats)
            {
                if (chat.ParentChatId != -1)
                {
                    var parentId = chat.ParentChatId;
                    if (parentId.HasValue && chatDict.ContainsKey(parentId.Value))
                    {
                        chatDict[parentId.Value].Children.Add(chatDict[chat.ChatId]);
                    }
                }
            }

            // Hàm sắp xếp đệ quy cho cây chat
            void SortChatNodes(List<ChatNode> nodes)
            {
                nodes.Sort((x, y) => y.Time.GetValueOrDefault().CompareTo(x.Time.GetValueOrDefault()));
                foreach (var node in nodes)
                {
                    if (node.Children.Any())
                    {
                        SortChatNodes(node.Children);
                    }
                }
            }

            // Sắp xếp danh sách rootChats và các node con
            SortChatNodes(rootChats);

            return rootChats;
        }
    }
}
