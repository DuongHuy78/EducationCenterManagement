using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Models;

namespace QuanLyTrungTamDaoTao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuanLyTrungTamDaoTaoContext _db;

        public HomeController(ILogger<HomeController> logger, QuanLyTrungTamDaoTaoContext context)
        {
            _logger = logger;
            _db = context;   
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Console.WriteLine(User.Identity.Name);
            }
            else
            {

            }
                return View();
        }

        #region  Hiển thị khóa học

        public async Task<IActionResult> HienThiKhoaHoc()
        {
            var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
            var danhSachKhoaHoc = await _db.KhoaHocs.Where(kh => kh.ThoiGianKhaiGiang > ngayHienTai)
                                                    .ToListAsync();
            return View(danhSachKhoaHoc);
        }
        #endregion
    }
}
