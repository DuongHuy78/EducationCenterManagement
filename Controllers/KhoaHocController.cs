using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Models;

namespace QuanLyTrungTamDaoTao.Controllers
{
    [Authorize("HocVien")]
    public class KhoaHocController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _db;

        public KhoaHocController(QuanLyTrungTamDaoTaoContext context)
        {
            _db = context;
        }

        #region  Hiển thị khóa học
        public async Task<IActionResult> Index()
        {
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

        #region Dang Ky Khoa Hoc
        public async Task<IActionResult> DangKyKhoaHocConfirmed(string id)
        {
            var KH = await _db.KhoaHocs.FirstOrDefaultAsync(kh => kh.MaKhoaHoc == id);
            if (KH!.SoLuongHocVienHienTai >= KH.SoLuongHocVienToiDa)
            {
                TempData["ErrorMessage"] = "Khóa học đã đủ người.";
                return RedirectToAction("Index");
            }

            var maHocVien = User.FindFirst("MaHocVien")?.Value;

            if (string.IsNullOrEmpty(maHocVien))
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra xem học viên đã đăng ký khóa học này chưa
            var dangKyHienTai = await _db.DangKyKhoaHocs
                .AnyAsync(dk => dk.MaKhoaHoc == id && dk.MaHocVien == maHocVien);

            if (dangKyHienTai)
            {
                // Học viên đã đăng ký khóa học này rồi
                TempData["ErrorMessage"] = "Bạn đã đăng ký khóa học này rồi.";
                return RedirectToAction("Index");
            }

            var lastDangKyKhoaHoc = await _db.DangKyKhoaHocs
                                    .OrderByDescending(kh => kh.MaDangKy)
                                    .FirstOrDefaultAsync();
            var newKhoaHocId = "";
            if (lastDangKyKhoaHoc == null)
            {
                newKhoaHocId = "DKKH0001";
            }
            else
            {
                string lastIdNumber = lastDangKyKhoaHoc.MaDangKy.Substring(4); // Bỏ "DKKH"
                int newIdNumber = int.Parse(lastIdNumber) + 1; // Tăng lên 1
                newKhoaHocId = "DKKH" + newIdNumber.ToString("D4"); // Định dạng lại với 4 chữ số
            }
            var newDangKyKhoaHoc = new DangKyKhoaHoc();
            newDangKyKhoaHoc.MaDangKy = newKhoaHocId;
            newDangKyKhoaHoc.MaKhoaHoc = id;
            newDangKyKhoaHoc.MaHocVien = User.FindFirst("MaHocVien")?.Value!;
            newDangKyKhoaHoc.NgayDangKy = DateOnly.FromDateTime(DateTime.Now);

            _db.Add(newDangKyKhoaHoc);

            KH.SoLuongHocVienHienTai += 1;

            _db.KhoaHocs.Update(KH);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bạn đã đăng ký thành công.";
            return RedirectToAction("Index");
        }
        #endregion

        #region Hủy khóa học
        public async Task<IActionResult> HuyDangKyKhoaHocConfirmed(string id)
        {
            var currentIdHocVien = User.FindFirst("MaHocVien")?.Value;
            if (string.IsNullOrEmpty(currentIdHocVien))
            {
                return RedirectToAction("DangNhap", "Account");
            }
            var KH = await _db.KhoaHocs.FirstOrDefaultAsync(kh => kh.MaKhoaHoc == id);
            var dangKyKhoaHoc = await _db.DangKyKhoaHocs
                .FirstOrDefaultAsync(dk => dk.MaKhoaHoc == id && dk.MaHocVien == currentIdHocVien);
            if (DateOnly.FromDateTime(DateTime.Now) >= dangKyKhoaHoc!.MaKhoaHocNavigation.ThoiGianKhaiGiang)
            {
                TempData["ErrorMessage"] = "Không thể hủy khi đang học.";
                return RedirectToAction("Index");
            }

            if (dangKyKhoaHoc != null)
            {
                KH!.SoLuongHocVienHienTai = KH.SoLuongHocVienHienTai - 1;
                if (KH.SoLuongHocVienHienTai <= 0) 
                    KH.SoLuongHocVienHienTai = 0;
                _db.KhoaHocs.Update(KH);
                _db.DangKyKhoaHocs.Remove(dangKyKhoaHoc);
                await _db.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Bạn đã hủy thành công.";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
