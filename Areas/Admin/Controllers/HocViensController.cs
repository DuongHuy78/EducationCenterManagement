using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Models;

namespace QuanLyTrungTamDaoTao.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class HocViensController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _context;

        public HocViensController(QuanLyTrungTamDaoTaoContext context)
        {
            _context = context;
        }

        #region Xem danh sách học viên
        public async Task<IActionResult> Index()
        {
            return View(await _context.HocViens.ToListAsync());
        }
        #endregion

        #region Danh sách khóa học học viên đã đăng ký
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
        #endregion

        #region Chỉnh sửa thông tin Học viên
        [HttpGet]
        public async Task<IActionResult> ChinhSuaThongTinHocVien(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }
            return View(hocVien);
        }

        [HttpPost]
        public async Task<IActionResult> ChinhSuaThongTinHocVien([Bind("MaHocVien,HoTen,NgaySinh,SoDienThoai,Email")] HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentHV = await _context.HocViens.FirstOrDefaultAsync(hv => hv.MaHocVien == hocVien.MaHocVien);

                    if(currentHV == null)
                    {
                        return NotFound();
                    }
                    if ((await _context.HocViens.AnyAsync(hv => hv.Email == hocVien.Email) && currentHV.Email != hocVien.Email)
                        || await _context.QuanTriViens.AnyAsync(qtv => qtv.Email == hocVien.Email))
                    {
                        TempData["ErrorMessage"] = "Email đã bị trùng";
                        return View(hocVien);
                    }

                    if ((await _context.HocViens.AnyAsync(hv => hv.SoDienThoai == hocVien.SoDienThoai) && currentHV.SoDienThoai != hocVien.SoDienThoai)
                        || await _context.QuanTriViens.AnyAsync(qtv => qtv.SoDienThoai == hocVien.SoDienThoai))
                    {
                        TempData["ErrorMessage"] = "Số điện thoại đã bị trùng";
                        return View(hocVien);
                    }
                    if(hocVien.HoTen == null || hocVien.NgaySinh == null || hocVien.Email == null || hocVien.SoDienThoai == null)
                    {
                        TempData["ErrorMessage"] = "Nhập đầy đủ thông tin";
                        return View(hocVien);
                    }

                    if (!Regex.IsMatch(hocVien.SoDienThoai, "^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$"))
                    {
                        TempData["ErrorMessage"] = "Số điện thoại định dạng không phù hợp";
                        return View(hocVien);
                    }

                    currentHV.HoTen = hocVien.HoTen;
                    currentHV.NgaySinh = hocVien.NgaySinh;
                    currentHV.SoDienThoai = hocVien.SoDienThoai;
                    currentHV.Email = hocVien.Email;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đã có lỗi " + ex.Message;
                }
                return RedirectToAction("Index", "HocViens");
            }
            return View(hocVien);
        }
        #endregion

        #region Xóa Học viên
        [HttpGet]
        public async Task<IActionResult> XoaHocVien(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }
            return View(hocVien);
        }

        [HttpPost]
        public async Task<IActionResult> XoaHocVienConfirm(string MaHocVien)
        {
            try
            {
                var currentHV = await _context.HocViens.FirstOrDefaultAsync(hv => hv.MaHocVien == MaHocVien);

                if (currentHV == null)
                {
                    return NotFound();
                }

                //nếu đã đăng ký thì không thể xóa học viên
                var daDangKy = await _context.DangKyKhoaHocs.AnyAsync(dk => dk.MaHocVien == MaHocVien);
                if(daDangKy)
                {
                    TempData["ErrorMessage"] = "Học viên đã đăng ký học không thể xóa.";
                    return RedirectToAction("Index", "HocViens");
                }

                var TaiKhoanHv = await _context.TaiKhoans.FirstOrDefaultAsync(tk => tk.MaNguoiDung == MaHocVien);
                if (TaiKhoanHv != null)
                {
                    _context.TaiKhoans.Remove(TaiKhoanHv);
                }
                var listMaKhoaHocDaDangKy = await _context.DangKyKhoaHocs
                        .Where(kh => kh.MaHocVien == MaHocVien)
                        .Select(dk => dk.MaKhoaHoc)
                        .ToListAsync();
                var DanhSachKhoaHocDaDangKy = await _context.KhoaHocs
                        .Where(kh => listMaKhoaHocDaDangKy.Contains(kh.MaKhoaHoc))
                        .ToListAsync();
                    
                _context.Remove(currentHV);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa thành công!";
                    
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi " + ex.Message;
            }
            return RedirectToAction("Index", "HocViens");
        }
        #endregion

        #region

        #endregion

    }
}
