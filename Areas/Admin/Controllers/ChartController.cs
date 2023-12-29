using projectdbfirst.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projectdbfirst.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    public class ChartController : Controller
    {
        private QLBHmvcEntities db = new QLBHmvcEntities();
        // GET: Admin/Chart
        public ActionResult Index()
        {
            var ordersData = db.HDNhaps
            .Select(o => new
            {
                Type = "HDNhaps",
                Month = o.NgayNhap.HasValue ? o.NgayNhap.Value.Month : 0,
                Year = o.NgayNhap.HasValue ? o.NgayNhap.Value.Year : 0,
                Amount = o.TongTien
            });

            var importsData = db.HDBans
                .Select(n => new
                {
                    Type = "HDBans",
                    Month = n.NgayBan.HasValue ? n.NgayBan.Value.Month : 0,
                    Year = n.NgayBan.HasValue ? n.NgayBan.Value.Year : 0,
                    Amount = n.TongTien
                });

            var allData = ordersData.Concat(importsData)
                .GroupBy(d => new { d.Month, d.Year })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    ImportAmount = g.FirstOrDefault(d => d.Type == "HDNhaps") != null ? g.FirstOrDefault(d => d.Type == "HDNhaps").Amount : 0,
                    OrderAmount = g.FirstOrDefault(d => d.Type == "HDBans") != null ? g.FirstOrDefault(d => d.Type == "HDBans").Amount : 0
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var labels = allData.Select(m => $"{m.Month}/{m.Year}").ToArray();
            var orderData = allData.Select(m => m.OrderAmount).ToArray();
            var importData = allData.Select(m => m.ImportAmount).ToArray();

            ViewBag.Labels = labels;
            ViewBag.OrderData = orderData;
            ViewBag.ImportData = importData;

            return View();
        }
        public ActionResult RemainingChart()
        {
            var productsData = db.Hangs
                .Select(h => new {
                    ProductName = h.TenH,
                    RemainingQuantity = h.SoLuong
                })
                .ToList();

            if (productsData != null && productsData.Any())
            {
                ViewBag.ProductsData = productsData;
            }
            else
            {
                ViewBag.ProductsData = new List<object>(); 
            }

            return View();
        }

        [HttpGet]
        public ActionResult ImportChart(int? thang, int? nam)
        {
            if(thang.HasValue && nam.HasValue)
            {
                // xác định các hóa đơn có tháng và năm
                List<Models.HDNhap> hdNhapData = db.HDNhaps.Where(c => c.NgayNhap.Value.Month == thang && c.NgayNhap.Value.Year == nam).ToList();

                // Dictionary để lưu trữ số lượng hàng nhập của từng sản phẩm
                Dictionary<string, int> productQuantities = new Dictionary<string, int>();

                // Lặp qua các hóa đơn và lấy các chi tiết hóa đơn kết nối với hóa đơn đó
                foreach(var hdNhap in hdNhapData)
                {
                    var ctHDNhapData = db.CTHDNhaps.Where(m => m.MaHDNhap == hdNhap.MaHDNhap).ToList();

                    // Tính tổng số lượng hàng nhập của từng sản phẩ
                    foreach(var cthdNhap in ctHDNhapData)
                    {
                        //string key = cthdNhap.MaH.ToString();
                        string key = db.Hangs.FirstOrDefault(m => m.MaH == cthdNhap.MaH)?.TenH;
                        if (productQuantities.ContainsKey(key))
                        {
                            productQuantities[key] += cthdNhap.SoLuong ?? 0;
                        }
                        else
                        {
                            productQuantities[key] = cthdNhap.SoLuong ?? 0;
                        }
                    }
                }
                // Chuyển dữ liệu từ Dictionary sang các mảng để sử dụng trong biểu đồ
                var productNames = productQuantities.Keys.ToArray();
                var remainingQuantities = productQuantities.Values.ToArray();

                //ViewBag.ProductNames = productNames;
                //ViewBag.RemainingQuantities = remainingQuantities;
                return Json(new { ProductNames = productNames, RemainingQuantities = remainingQuantities }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [HttpGet]
        public ActionResult OrderChart(int? thang, int? nam)
        {
            if (thang.HasValue && nam.HasValue)
            {
                // xác định các hóa đơn có tháng và năm
                List<Models.HDBan> hdBanData = db.HDBans.Where(c => c.NgayBan.Value.Month == thang && c.NgayBan.Value.Year == nam).ToList();

                // Dictionary để lưu trữ số lượng hàng nhập của từng sản phẩm
                Dictionary<string, int> productQuantities = new Dictionary<string, int>();

                // Lặp qua các hóa đơn và lấy các chi tiết hóa đơn kết nối với hóa đơn đó
                foreach (var hdBan in hdBanData)
                {
                    var ctHDBanData = db.CTHDBans.Where(m => m.MaHDBan == hdBan.MaHDBan).ToList();

                    // Tính tổng số lượng hàng nhập của từng sản phẩ
                    foreach (var cthdBan in ctHDBanData)
                    {
                        //string key = cthdNhap.MaH.ToString();
                        string key = db.Hangs.FirstOrDefault(m => m.MaH == cthdBan.MaH)?.TenH;
                        if (productQuantities.ContainsKey(key))
                        {
                            productQuantities[key] += cthdBan.SoLuong ?? 0;
                        }
                        else
                        {
                            productQuantities[key] = cthdBan.SoLuong ?? 0;
                        }
                    }
                }
                // Chuyển dữ liệu từ Dictionary sang các mảng để sử dụng trong biểu đồ
                var productNames = productQuantities.Keys.ToArray();
                var remainingQuantities = productQuantities.Values.ToArray();

                //ViewBag.ProductNames = productNames;
                //ViewBag.RemainingQuantities = remainingQuantities;
                return Json(new { ProductNames = productNames, RemainingQuantities = remainingQuantities }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
    }
}