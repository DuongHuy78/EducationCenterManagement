﻿using AutoMapper; // Add AutoMapper
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add EF Core for async operations
using QuanLyTrungTamDaoTao.Data;
using QuanLyTrungTamDaoTao.ViewModels;
using QuanLyTrungTamDaoTao.Helper;
using System.Linq; // Add Linq
using System.Threading.Tasks; // Add Task for async

namespace QuanLyTrungTamDaoTao.Controllers
{
    public class Register : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext db;
        private readonly IMapper _mapper;

        public Register(QuanLyTrungTamDaoTaoContext context, IMapper mapper) {
            db = context;
            _mapper = mapper; // Assign mapper
        }

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
                Console.WriteLine(" thành công");
               try
               {
                   if (db.HocViens.Any(h => h.Email == model.Email))
                   {
                       ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                       return View(model);
                   }
                   if (db.TaiKhoans.Any(t => t.TenTaiKhoan == model.TaiKhoan))
                   {
                       ModelState.AddModelError("TaiKhoan", "Tài khoản này đã được sử dụng.");
                       return View(model);
                   }

                   var LastHV = await db.HocViens
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

                   db.HocViens.Add(newHV);
                   await db.SaveChangesAsync();

                    var newTK = new TaiKhoan();
                    newTK.TenTaiKhoan = model.TaiKhoan;
                    newTK.MatKhau = model.MatKhau;
                    newTK.MaNguoiDung = newMaHV;
                    newTK.VaiTro = "HV";
                    db.TaiKhoans.Add(newTK);
                    await db.SaveChangesAsync();

                    return View("Index");
               }
               catch (Exception ex)
               {
                   ModelState.AddModelError("", "Đã có lỗi xảy ra trong quá trình đăng ký.");
               }
            }
            else
            {
                ModelState.AddModelError("New Error", "Invalueable");
            }
           return View();
        }
    }
}
