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

        // GET: Admin/QuanLyKhoaHoc
        #region Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhoaHocs.ToListAsync());
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(string id)
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
                _context.Add(khoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khoaHoc);
        }
        #endregion

        #region Edit Khóa học
        // GET: Admin/HomeAdmin/Edit/5
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKhoaHoc,TenKhoaHoc,GiangVien,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoaHoc);
                    await _context.SaveChangesAsync();
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

        // POST: Admin/HomeAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc != null)
            {
                _context.KhoaHocs.Remove(khoaHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion
        private bool KhoaHocExists(string id)
        {
            return _context.KhoaHocs.Any(e => e.MaKhoaHoc == id);
        }
    }
}
