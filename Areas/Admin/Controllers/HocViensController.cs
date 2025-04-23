using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Models;

namespace QuanLyTrungTamDaoTao.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HocViensController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _context;

        public HocViensController(QuanLyTrungTamDaoTaoContext context)
        {
            _context = context;
        }

        // GET: Admin/HocViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.HocViens.ToListAsync());
        }

        public async Task<IActionResult> DanhSachKhoaHoc(string id)
        {
            var listMaKhoaHocDaDangKy = await _context.DangKyKhoaHocs
                .Where(kh => kh.MaHocVien == id)
                .Select(dk => dk.MaKhoaHoc)
                .ToListAsync();
            var DanhSachKhoaHocDaDangKy = await _context.KhoaHocs
                        .Where(kh => listMaKhoaHocDaDangKy.Contains(kh.MaKhoaHoc))
                        .ToListAsync();
            return View(DanhSachKhoaHocDaDangKy);
        }

        private bool HocVienExists(string id)
        {
            return _context.HocViens.Any(e => e.MaHocVien == id);
        }
    }
}
