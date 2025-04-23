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
using Microsoft.AspNetCore.Authorization;

namespace QuanLyTrungTamDaoTao.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "QTV")]
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

            var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
            var ngayBatDau = ngayHienTai.AddMonths(-5);

            // danh sách tất cả 6 tháng gần đây
            var monthList = new List<MonthlyRegistration>();
            for (int i = 0; i <= 5; i++)
            {
                var month = ngayHienTai.AddMonths(-i);
                monthList.Add(new MonthlyRegistration 
                {
                    Year = month.Year,
                    Month = month.Month,
                    Count = 0,
                    DoanhThu = 0m
                });
            }
            

            // Truy vấn đăng ký doanh thu trong 6 tháng gần đây
            var dangKyTrong6Thang = await _context.DangKyKhoaHocs
                        .Where(dk => dk.NgayDangKy >= ngayBatDau && dk.NgayDangKy <= ngayHienTai)
                        .GroupBy(dk => new { dk.NgayDangKy.Year, dk.NgayDangKy.Month })
                        .Select(g => new MonthlyRegistration
                        {
                            Month = g.Key.Month,
                            Year = g.Key.Year,
                            Count = g.Count(),
                            DoanhThu = g.Sum(dk => dk.MaKhoaHocNavigation.HocPhi)
                        })
                        .ToListAsync();

            // Kết hợp dữ liệu để đảm bảo tất cả các tháng đều có (kể cả tháng không có đăng ký)
            var combinedData = monthList.Select(m => 
            {
                var existingData = dangKyTrong6Thang
                    .FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month);
                
                return existingData != null ? existingData.Count : 0;
            }).Reverse().ToArray(); // Đảo ngược để hiển thị từ tháng xa nhất đến gần nhất

            // Tạo mảng doanh thu theo tháng
            var revenueData = monthList.Select(m => 
            {
                var existingData = dangKyTrong6Thang
                    .FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month);
                
                return existingData != null ? existingData.DoanhThu : 0m;
            }).Reverse().ToArray();

            // Tổng doanh thu
            var totalDoanhThu = 0m;
            foreach (var khoaHoc in listKhoaHoc)
            {
                totalDoanhThu += khoaHoc.SoLuongHocVienHienTai * khoaHoc.HocPhi;
            }

            ViewBag.TotalKhoaHoc = totalkhoaHoc;
            ViewBag.TotalHocVien = totalHocVien;
            ViewBag.TotalDoanhThu = totalDoanhThu;
            ViewBag.DangKyData = combinedData; // Mảng số lượng đăng ký theo tháng
            ViewBag.DoanhThuData = revenueData; // Mảng doanh thu theo tháng
            
            // Danh sách tên tháng để hiển thị trên biểu đồ
            ViewBag.MonthLabels = monthList.Select(m => $"{m.Month}/{m.Year}").Reverse().ToArray();
            return View();
        }

        // Lớp hỗ trợ
        public class MonthlyRegistration
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Count { get; set; }
            public decimal DoanhThu { get; set; }
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
