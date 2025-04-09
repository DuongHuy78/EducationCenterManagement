using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamDaoTao.ViewModels
{
    public class Register
    {
        [Display(Name = "Mã học viên")]
        public string MaHocVien { get; set; }

        [Display(Name = "Họ và tên")]
        [MaxLength(255)]
        public string HoTen { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateOnly? NgaySinh { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Số Diện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Tài khoản")]
        public string? TaiKhoan { get; set; }

        [Display(Name = "Mật khẩu")]
        public string? MatKhau { get; set; }
    }
}
