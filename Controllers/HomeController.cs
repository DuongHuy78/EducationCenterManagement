using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        #region  Hiển thị khóa học
        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (User.IsInRole("QTV"))
                {
                    return RedirectToAction("Dashboard", "HomeAdmin", new { area = "Admin" });
                }
            }

            var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
            var danhSachKhoaHoc = await _db.KhoaHocs.Where(kh => kh.ThoiGianKhaiGiang > ngayHienTai)
                                                    .ToListAsync();

            var maHocVien = User.FindFirst("MaHocVien")?.Value;
            var listMaKhoaHocDaDangKy = await _db.DangKyKhoaHocs
                .Where(kh => kh.MaHocVien == maHocVien)
                .Select(dk => dk.MaKhoaHoc)
                .ToListAsync();
            var DanhSachKhoaHocDaDangKy = await _db.KhoaHocs
                        .Where(kh => listMaKhoaHocDaDangKy.Contains(kh.MaKhoaHoc))
                        .ToListAsync();
            ViewBag.DanhSachKhoaHocDaDangKy = DanhSachKhoaHocDaDangKy;
            return View(danhSachKhoaHoc);
        }
        #endregion
    }
}
