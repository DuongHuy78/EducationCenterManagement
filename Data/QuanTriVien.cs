using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Data;

public partial class QuanTriVien
{
    public int MaQuanTriVien { get; set; }

    public string TenQuanTriVien { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string Email { get; set; } = null!;
}
