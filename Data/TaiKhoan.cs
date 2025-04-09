using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Data;

public partial class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public string TenTaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public int MaNguoiDung { get; set; }
}
