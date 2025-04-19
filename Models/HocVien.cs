using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Models;

public partial class HocVien
{
    public string MaHocVien { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();
}
