using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamDaoTao.Models;
using System.Security.Claims;

namespace QuanLyTrungTamDaoTao.Controllers
{
    [Authorize("HocVien")]
    public class ProfileController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _context;
        public ProfileController(QuanLyTrungTamDaoTaoContext context)
        {
            _context = context;
        }

        #region
        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            var maHocVien = User.FindFirst("MaHocVien")?.Value;
            if (maHocVien == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(maHocVien);
            if (hocVien == null)
            {
                return NotFound();
            }
            return View(hocVien);
        }
        #endregion

        #region Chỉnh xửa thông tin cá nhân
        [HttpGet]
        public async Task<IActionResult> ChinhSuaThongTinCaNhan()
        {
            var maHocVien = User.FindFirst("MaHocVien")?.Value;
            if (maHocVien == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(maHocVien);
            if (hocVien == null)
            {
                return NotFound();
            }
            return View(hocVien);
        }

        [HttpPost]
        public async Task<IActionResult> ChinhSuaThonTinCaNhan([Bind("MaHocVien,HoTen,NgaySinh,SoDienThoai,Email")] HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var maHocVien = User.FindFirst("MaHocVien")?.Value;
                    if (maHocVien == null)
                    {
                        return NotFound();
                    }

                    if(hocVien.MaHocVien != maHocVien)
                    {
                        return NotFound();
                    }

                    var currentHV = await _context.HocViens.FirstAsync(hv => hv.MaHocVien == hocVien.MaHocVien);

                    if (currentHV == null)
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

                    currentHV.HoTen = hocVien.HoTen;
                    currentHV.NgaySinh = hocVien.NgaySinh;
                    currentHV.SoDienThoai = hocVien.SoDienThoai;
                    currentHV.Email = hocVien.Email;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

                    // Cập nhật lại cookie xác thực với tên mới
                    var claims = User.Claims.ToList();

                    // Tìm và cập nhật claim Name
                    var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    if (nameClaim != null)
                    {
                        claims.Remove(nameClaim);
                        claims.Add(new Claim(ClaimTypes.Name, hocVien.HoTen));

                        var identity = new ClaimsIdentity(claims, "Cookie");
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(
                            "Cookies",
                            principal,
                            new AuthenticationProperties { IsPersistent = true });
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đã có lỗi " + ex.Message;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(hocVien);
        }
        #endregion
    }
}
