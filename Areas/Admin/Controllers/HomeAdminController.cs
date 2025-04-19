using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Models;

namespace QuanLyTrungTamDaoTao.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _context;

        public HomeAdminController(QuanLyTrungTamDaoTaoContext context)
        {
            _context = context;
        }

        #region Dashboard

        public async Task<IActionResult> Dashboard()
        {
            var totalkhoaHoc = _context.KhoaHocs.Count();
            var totalHocVien = _context.HocViens.Count();
            var listKhoaHoc = _context.KhoaHocs.AsNoTracking().ToList();
            var ngayHienTai = DateTime.Now;
            var ngayBatDau = ngayHienTai.AddMonths(-5);
            var dangKyTrong6Thang = await _context.DangKyKhoaHocs
                    .Where(dk => dk.NgayDangKy.HasValue && dk.NgayDangKy >= ngayBatDau && dk.NgayDangKy <= ngayHienTai)
                    .GroupBy(dk => new { dk.NgayDangKy.Value.Year, dk.NgayDangKy.Value.Month })
                    .Select(g => new
                    {
                        Thang = g.Key.Month,
                        Nam = g.Key.Year,
                        SoLuongDangKy = g.Count()
                    })
                    .OrderBy(g => g.Nam).ThenBy(g => g.Thang)
                    .ToListAsync();

            // Sử dụng decimal để tính toán chính xác
            var totalDoanhThu = 0m;
            foreach (var khoaHoc in listKhoaHoc)
            {
                totalDoanhThu += khoaHoc.SoLuongHocVienHienTai * khoaHoc.HocPhi;
            }
            var temp = new List<int> { 10, 20, 30, 40, 50, 60 };


            ViewBag.TotalKhoaHoc = totalkhoaHoc;
            ViewBag.TotalHocVien = totalHocVien;
            ViewBag.TotalDoanhThu = totalDoanhThu;
            //ViewBag.dangKyTrong6Thang = dangKyTrong6Thang;
            ViewBag.dangKyTrong6Thang = temp;
            return View();
        }
        #endregion

        #region Dang Xuat
        [HttpGet]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("DangNhap", "Account", new { area = "" });
        }
        #endregion

        private bool KhoaHocExists(string id)
        {
            return _context.KhoaHocs.Any(e => e.MaKhoaHoc == id);
        }
    }
}
