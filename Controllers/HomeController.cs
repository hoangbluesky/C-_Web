using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;
using System.Threading;
using System.Timers;
using System.Web.Services.Description;

namespace projectdbfirst.Controllers
{
    //[Authorize(Roles = "admin, client")]
    public class HomeController : Controller
    {
    Models.QLBHmvcEntities db = new Models.QLBHmvcEntities();
        // GET: home
        public ActionResult Index()
        {
            ViewBag.checkAdmin = false;
            if (HttpContext.Session["AdminId"] != null)
            {
                ViewBag.checkAdmin = true;
            }
            List<Models.Hang> data = db.Hangs.Take(10).ToList();
            ViewBag.Data = data;
            return View();
        }

        public ActionResult Details(int? id)
        {
            List<Models.Hang> data = db.Hangs.Take(5).ToList();
            ViewBag.Data = data;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Hang hang = db.Hangs.Find(id);
            if (hang == null)
            {
                return HttpNotFound();
            }

            // chat: start

            var chats = db.Chats.Where(c => c.MaH == id).ToList();
            var chatTree = BuildChatTree(chats);

            ViewBag.ChatTree = chatTree;

            // chat: end
            return View(hang);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            // Kiểm tra xem Session "Cart" có tồn tại không
            if (Session["Cart"] != null)
            {
                List<Models.CartModel> cartItems = (List<Models.CartModel>)Session["Cart"];

                // Tìm sản phẩm trong giỏ hàng với productId cần xóa
                Models.CartModel productToRemove = cartItems.FirstOrDefault(item => item.ProductID == productId);

                if (productToRemove != null)
                {
                    // Xóa sản phẩm khỏi giỏ hàng
                    cartItems.Remove(productToRemove);

                    // Cập nhật Session "Cart" sau khi xóa
                    Session["Cart"] = cartItems;
                }
            }

            // Sau khi xóa, có thể chuyển hướng người dùng đến trang giỏ hàng hoặc trang tương tự
            return RedirectToAction("Index");
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Shop(int? page)
        {
            var lst = db.Hangs.ToList();
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            IPagedList<Hang> pageResult = lst.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }
        [HttpPost]
        public ActionResult Shop(string search, int? page)
        {
            var lst = db.Hangs.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                lst = lst.Where(m => m.TenH.Contains(search)).ToList();
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            IPagedList<Hang> pageResult = lst.ToPagedList(pageNumber, pageSize);
            return View(pageResult);
        }

        [HttpPost]
        public JsonResult Reply(int parentId, string content, int maH)
        {
            if( content != null)
            {
                try
                {
                    int id = (int)Session["UserId"];
                    //int id = 2;
                    var kh = db.KHs.FirstOrDefault(k => k.KHUserId == id);
                    var chat = new Chat
                    {
                        MaKH = kh.MaKH, // Sử dụng mã khách hàng thực tế
                        MaH = maH,
                        ParentChatId = parentId,
                        Content = content,
                        Time = DateTime.Now // Gán thời gian hiện tại
                    };

                    db.Chats.Add(chat);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung phần bình luận" });
            }
        }
        // class chat
        public class ChatNode
        {
            public int ChatId { get; set; }
            public string TenKH { get; set; }
            public int? MaH { get; set; }
            public int? ParentChatId { get; set; }
            public string Content { get; set; }
            public DateTime? Time { get; set; } // Thời gian gửi phản hồi
            public string TimeAgo { get; set; } // Chuỗi khoảng thời gian
            public List<ChatNode> Children { get; set; }

            public ChatNode()
            {
                Children = new List<ChatNode>(); // Khởi tạo danh sách
            }
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