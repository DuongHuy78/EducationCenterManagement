using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.Identity.Client;
using QuanLyTrungTamDaoTao.Data;

namespace QuanLyTrungTamDaoTao.Controllers
{
    public class Register : Controller
    {
        private readonly QuanLyTrungTamDaoTaoContext db;
        public Register(QuanLyTrungTamDaoTaoContext context) {
            db = context;
        }
        public IActionResult DangKy()
        {
            return View();
        }
    }
}
