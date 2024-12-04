using Microsoft.Ajax.Utilities;
using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VNPAY_CS_ASPX;

namespace projectdbfirst.Controllers
{
    [Authorize(Roles = "client")]
    public class CartController : Controller
    {
        Models.QLBHmvcEntities db = new Models.QLBHmvcEntities();
        // GET: Cart
        public ActionResult Index()
        {
            if (TempData["SuccessMessageBuy"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessageBuy"].ToString();
            }
            if (TempData["ErrorMessageBuy"] != null)
            {
                ViewBag.ErrorMessageBuy = TempData["ErrorMessageBuy"].ToString();
            }
            if (Session["Cart"] == null)
            {
                if (TempData["ErrorMessageBuy"] != null || TempData["SuccessMessageBuy"] != null)
                    return View();
                else
                    ViewBag.SessionNull = "Giỏ hàng của bạn đang rỗng!";
            }
            return View();
        }

        public ActionResult AddToCart(int id)
        {
            List<Models.CartModel> lstCart = null;
            if (Session["Cart"] == null)
            {
                lstCart = new List<Models.CartModel>();
            }
            else
            {
                lstCart = (List<Models.CartModel>)Session["Cart"];
            }
            Models.CartModel obj = lstCart.FirstOrDefault(m => m.ProductID == id);
            if (obj == null)
            {
                Models.Hang crrProduct = db.Hangs.First(m => m.MaH == id);
                //if(crrProduct.GiamGia > 0)
                //{
                //    double a = (double)crrProduct.GiaTien;
                //    crrProduct.GiaTien = (double)(a * (100 - crrProduct.GiamGia) / 100);
                //}
                obj = new Models.CartModel
                {
                    ProductID = id,
                    ProductDetail = crrProduct,
                    Quantity = 1,
                    Amount = (double)(1 * (crrProduct.GiaTien * (100 - crrProduct.GiamGia) / 100)),
                    Note = "",
                };
                lstCart.Add(obj);
            }
            else
            {
                obj.Quantity = obj.Quantity + 1;
                obj.Amount = (double)(obj.Quantity * (obj.ProductDetail.GiaTien * (100 - obj.ProductDetail.GiamGia) / 100));
            }
            Session["Cart"] = lstCart;
            return View("Index");
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

        public ActionResult DeleteAll()
        {
            Session["Cart"] = null;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateCart() 
        {

            return RedirectToAction("Index");
        }
        public void savehd()
        {
            List<Models.CartModel> lst = (List<Models.CartModel>)Session["Cart"];

            // create HDBan
            projectdbfirst.Models.HDBan hdBan = new projectdbfirst.Models.HDBan();
            double tongtien = 0;
            // get current date
            //DateTime currentDate = DateTime.Now;
            //string formattedDate = currentDate.ToString("MM/dd/yyyy");
            // get MaKH
            int id = (int)Session["UserId"];
            //int id = 2;
            var kh = db.KHs.FirstOrDefault(m => m.KHUserId == id);
            hdBan.NgayBan = DateTime.Now;
            hdBan.MaKH = kh.MaKH;
            hdBan.TongTien = tongtien;

            db.HDBans.Add(hdBan);
            db.SaveChanges();

            // create all CTHDBan

            List<projectdbfirst.Models.CTHDBan> cthdBanList = new List<projectdbfirst.Models.CTHDBan>();
            // get MaHDBan
            int newMaHDBan = hdBan.MaHDBan;
            for (int i = 0; i < lst.Count; i++)
            {
                projectdbfirst.Models.CTHDBan ctHDBan = new projectdbfirst.Models.CTHDBan();
                ctHDBan.MaHDBan = newMaHDBan;
                ctHDBan.MaH = lst[i].ProductDetail.MaH;
                ctHDBan.SoLuong = lst[i].Quantity;
                ctHDBan.DonGia = lst[i].ProductDetail.GiaTien;
                ctHDBan.GiamGia = lst[i].ProductDetail.GiamGia;
                ctHDBan.ThanhTien = lst[i].Amount;

                tongtien += lst[i].Amount;

                cthdBanList.Add(ctHDBan);
            }
            // update SoLuong in Hang
            foreach (var ctHDBan in cthdBanList)
            {
                var hangToUpdate = db.Hangs.FirstOrDefault(c => c.MaH == ctHDBan.MaH);
                if (hangToUpdate != null)
                {
                    hangToUpdate.SoLuong -= ctHDBan.SoLuong;
                }
            }
            db.SaveChanges();

            db.CTHDBans.AddRange(cthdBanList);
            db.SaveChanges();

            // update tongtien in HDBan
            hdBan.TongTien = tongtien;
            db.SaveChanges();

            Session["Cart"] = null;
            
        }

        public ActionResult BuyProduct()
        {
            return View();
        }
        public ActionResult Onepay()
        {
            int id = (int)Session["UserId"];
            var kh = db.KHs.FirstOrDefault(m => m.KHUserId == id);
            ViewBag.FullName = kh.FullName;
            ViewBag.DiaChi = kh.DiaChi;
            return View();
        }

        [HttpPost]
        public ActionResult Twopay(string TypePaymentVN)
        {
            int typePaymentVNInt = int.Parse(TypePaymentVN);
            var code = new { Success = true, Code = typePaymentVNInt, Url = "" };
            List<Models.CartModel> lst = (List<Models.CartModel>)Session["Cart"];
            int id = (int)Session["UserId"];
            var url = UrlPayment(lst, 2, id); // important
            code = new { Success = true, Code = typePaymentVNInt, Url = url }; // important
            return Redirect(code.Url);
        }

        public string UrlPayment(List<Models.CartModel> lst, int typementVn, int id)
        {
            // Tính tổng tiền
            double tongtien = 0;
            for ( int i = 0; i < lst.Count; i++)
            {
                tongtien += lst[i].Amount;
            }
            var kh = db.KHs.FirstOrDefault(k => k.KHUserId == id);

            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Get payment input



            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (tongtien * 100000).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (typementVn == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (typementVn == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (typementVn == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + kh.FullName);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", id.ToString());
            // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];

                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100; // chú ý phần giá tiền thanh toán ở đây

                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        savehd();
                        TempData["SuccessMessageBuy"] = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        TempData["ErrorMessageBuy"] = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}