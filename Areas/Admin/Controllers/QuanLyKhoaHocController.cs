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
    public class QuanLyKhoaHocController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _context;

        public QuanLyKhoaHocController(QuanLyTrungTamDaoTaoContext context)
        {
            _context = context;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhoaHocs.ToListAsync());
        }
        #endregion

        #region Kết thúc khóa học
        public async Task<IActionResult> End(string id)
        {

            if (id == null)
            {
                TempData["ErrorMessage"] = "Đã gặp vấn đề";
                return RedirectToAction("Index");
            }

            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                TempData["ErrorMessage"] = "Đã gặp vấn đề";
                return RedirectToAction("Index");
            }

            if(khoaHoc.ThoiGianKhaiGiang >= DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["ErrorMessage"] = "Không thể kết thúc khóa học khi chưa bắt đầu.";
                return RedirectToAction("Index");
            }

            khoaHoc.ThoiGianKetThuc = DateOnly.FromDateTime(DateTime.Now);
            _context.Update(khoaHoc);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bạn đã kết thúc khóa học thành công";
            return RedirectToAction("Index");
        }
        #endregion

        #region Create Khóa học
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var lastKhoaHoc =  await _context.KhoaHocs
                                    .OrderByDescending(kh => kh.MaKhoaHoc)
                                    .FirstOrDefaultAsync();
            var newKhoaHocId = "";
            if (lastKhoaHoc == null)
            {
                newKhoaHocId = "KH0001";
            }
            else
            {
                string lastIdNumber = lastKhoaHoc.MaKhoaHoc.Substring(2); // Bỏ "KH"
                int newIdNumber = int.Parse(lastIdNumber) + 1; // Tăng lên 1
                newKhoaHocId = "KH" + newIdNumber.ToString("D4"); // Định dạng lại với 4 chữ số
            }
            ViewBag.NewKhoaHocId = newKhoaHocId;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("MaKhoaHoc,TenKhoaHoc,GiangVien,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc)
        {
            if (ModelState.IsValid)
            {
                if(khoaHoc.ThoiGianKetThuc <= khoaHoc.ThoiGianKhaiGiang)
                {
                    TempData["ErrorMessage"] = "Thời gian khai giảng và kết thúc không phù hợp.";
                    return View(khoaHoc);
                }
                _context.Add(khoaHoc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bạn đã tạo thành công.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Tạo không thành công.";
            return View(khoaHoc);
        }
        #endregion

        #region Edit Khóa học
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            return View(khoaHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKhoaHoc,TenKhoaHoc,GiangVien,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc)
            {
                return NotFound();
            }

            if (ModelState.IsValid )
            {
                if (khoaHoc.ThoiGianKetThuc <= khoaHoc.ThoiGianKhaiGiang)
                {
                    TempData["ErrorMessage"] = "Thời gian khai giảng và kết thúc không phù hợp.";
                    return View(khoaHoc);
                }
                try
                {
                    _context.Update(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Bạn đã chỉnh sửa thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaHocExists(khoaHoc.MaKhoaHoc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(khoaHoc);
        }
        #endregion

        #region Delete khóa học
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            return View(khoaHoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khóa học.";
                return RedirectToAction("Index");
            }

            var listKhoaHocDaDangKy = await _context.DangKyKhoaHocs.Where(kh => kh.MaKhoaHoc == id)
                                                                    .ToListAsync();

            if (khoaHoc.ThoiGianKhaiGiang > DateOnly.FromDateTime(DateTime.Now))
            {
                foreach(DangKyKhoaHoc dangKy in listKhoaHocDaDangKy)
                {
                    _context.DangKyKhoaHocs.Remove(dangKy);
                }
            }
            else
            {
                if (listKhoaHocDaDangKy != null && listKhoaHocDaDangKy.Count > 0)
                {
                    TempData["ErrorMessage"] = "Khóa học đã bắt đầu và có người học, không được xóa";
                    return RedirectToAction("Index");

                }
            }

            _context.KhoaHocs.Remove(khoaHoc);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bạn đã xóa thành công.";
            return RedirectToAction(nameof(Index));
        }
        #endregion
        private bool KhoaHocExists(string id)
        {
            return _context.KhoaHocs.Any(e => e.MaKhoaHoc == id);
        }
    }
}
