using System.ComponentModel.DataAnnotations;
namespace QuanLyTrungTamDaoTao.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public string userName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string password { get; set; }

    }
}
