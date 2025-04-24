using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add EF Core for async operations
using QuanLyTrungTamDaoTao.ViewModels;
using QuanLyTrungTamDaoTao.Helper;
using System.Linq; // Add Linq
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuanLyTrungTamDaoTao.Models; // Add Task for async

namespace QuanLyTrungTamDaoTao.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext _db;

        public AccountController(QuanLyTrungTamDaoTaoContext context) {
            _db = context;
        }

        #region DangKy
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(RegisterVM model)
        {
            //Console.WriteLine(model.NgaySinh);
            //Console.WriteLine(model.MatKhau);
            //Console.WriteLine(model.HoTen);
            if (ModelState.IsValid)
            {
               try
               {
                   if (_db.HocViens.Any(h => h.Email == model.Email))
                   {
                       ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                       return View(model);
                   }
                   if (_db.TaiKhoans.Any(t => t.TenTaiKhoan == model.TaiKhoan))
                   {
                       ModelState.AddModelError("TaiKhoan", "Tài khoản này đã được sử dụng.");
                       return View(model);
                   }
                    if (_db.HocViens.Any(t => t.SoDienThoai == model.SoDienThoai) || _db.QuanTriViens.Any(t => t.SoDienThoai == model.SoDienThoai))
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng.");
                        return View(model);
                    }

                    var LastHV = await _db.HocViens
                                        .OrderByDescending(HV => HV.MaHocVien)
                                        .FirstOrDefaultAsync();
                    string newMaHV;
                    if (LastHV == null)
                    {
                        newMaHV = "HV0001";
                    }
                    else
                    {
                        int temp = int.Parse(LastHV.MaHocVien.Substring(2));
                        temp++;
                        newMaHV = "HV" + temp.ToString("D4");
                    }

                    var newHV = new HocVien();
                    newHV.MaHocVien = newMaHV;
                    newHV.HoTen = model.HoTen;
                    newHV.NgaySinh = model.NgaySinh;
                    newHV.Email = model.Email;
                    newHV.SoDienThoai = model.SoDienThoai;

                    _db.HocViens.Add(newHV);
                    await _db.SaveChangesAsync();

                    var newTK = new TaiKhoan();
                    newTK.TenTaiKhoan = model.TaiKhoan;
                    newTK.MatKhau = Utils.hashPassword(model.MatKhau);
                    newTK.MaNguoiDung = newMaHV;
                    newTK.VaiTro = "HV";
                    _db.TaiKhoans.Add(newTK);
                    await _db.SaveChangesAsync();

                    //-------------------------đăng ký cho quản trị viên-----------------------------------------
                    //var LastQTV = await _db.QuanTriViens
                    // .OrderByDescending(HV => HV.MaQuanTriVien)
                    // .FirstOrDefaultAsync();
                    //string newMaHV;
                    //if (LastQTV == null)
                    //{
                    //    newMaHV = "QTV0001";
                    //}
                    //else
                    //{
                    //    int temp = int.Parse(LastQTV.MaQuanTriVien.Substring(2));
                    //    temp++;
                    //    newMaHV = "QTV" + temp.ToString("D4");
                    //}

                    //var newQTV = new QuanTriVien();
                    //newQTV.MaQuanTriVien = newMaHV;
                    //newQTV.TenQuanTriVien = model.HoTen;
                    //newQTV.SoDienThoai = model.SoDienThoai;
                    //newQTV.Email = model.Email;

                    //_db.QuanTriViens.Add(newQTV);
                    //await _db.SaveChangesAsync();

                    //var newTK = new TaiKhoan();
                    //newTK.TenTaiKhoan = model.TaiKhoan;
                    //newTK.MatKhau = Utils.hashPassword(model.MatKhau);
                    //newTK.MaNguoiDung = newMaHV;
                    //newTK.VaiTro = "QTV";
                    //_db.TaiKhoans.Add(newTK);
                    //await _db.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
               catch (Exception ex)
               {
                   ModelState.AddModelError("", $"Đã có lỗi xảy ra trong quá trình đăng ký. {ex.Message}");
               }
            }
            else
            {
                ModelState.AddModelError("New Error", "Invalueable");
            }
            return View();
        }
        #endregion

        #region Dang Nhap
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.Values.SelectMany(v => v.Errors).ToList().ForEach(error =>
                    Console.WriteLine("Lỗi ModelState: " + error.ErrorMessage));
                return View(model);
            }

            var user = _db.TaiKhoans.SingleOrDefault(user => user.TenTaiKhoan == model.userName);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Tài khoản không tồn tại";
            }
            else
            {
                if (user.MatKhau != Utils.hashPassword(model.password))
                {
                    Console.WriteLine(Utils.hashPassword(model.password));
                    Console.WriteLine(Utils.hashPassword(model.password));
                    ModelState.AddModelError("loi", "Sai mật khẩu");
                }
                else
                {
                    Console.WriteLine(user.VaiTro);
                    List<Claim> claims = new List<Claim>();
                    if (user.VaiTro == "HV")
                    {
                        var hocVien = _db.HocViens.SingleOrDefault(hv => hv.MaHocVien == user.MaNguoiDung);

                        claims.Add(new Claim(ClaimTypes.Name, hocVien!.HoTen));
                        claims.Add(new Claim(ClaimTypes.Email, hocVien.Email!));
                        claims.Add(new Claim("MaHocVien", hocVien.MaHocVien));
                        claims.Add(new Claim(ClaimTypes.Role, "HV"));

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        try
                        {
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            Console.WriteLine("Đăng nhập thành công");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi SignInAsync: " + ex.Message);
                            return View(model);
                        }
                    }
                    else if(user.VaiTro == "QTV")
                    {
                        var qtv = _db.QuanTriViens.SingleOrDefault(QTV => QTV.MaQuanTriVien == user.MaNguoiDung);
                        Console.WriteLine(qtv!.TenQuanTriVien);
                        claims.Add(new Claim(ClaimTypes.Name, qtv.TenQuanTriVien));
                        claims.Add(new Claim(ClaimTypes.Email, qtv.Email));
                        claims.Add(new Claim("MaQuanTriVien", qtv.MaQuanTriVien));
                        claims.Add(new Claim(ClaimTypes.Role, "QTV"));

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        try
                        {
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            TempData["SuccessMessage"] = "Đăng nhập thành công";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi SignInAsync: " + ex.Message);
                            return View(model);
                        }

                        return RedirectToAction("Dashboard", "HomeAdmin", new { area = "Admin" });
                           
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Vai trò không phù hợp";
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Dang Xuat
        [HttpGet]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
