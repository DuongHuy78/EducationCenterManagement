using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamDaoTao.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [MaxLength(255, ErrorMessage = "Họ và tên không được vượt quá 255 ký tự.")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateOnly NgaySinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; } = null!;

        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public string TaiKhoan { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = null!;
    }


}
