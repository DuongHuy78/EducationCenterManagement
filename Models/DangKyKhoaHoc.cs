using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Models;

public partial class DangKyKhoaHoc
{
    public string MaDangKy { get; set; } = null!;

    public string MaHocVien { get; set; } = null!;

    public string MaKhoaHoc { get; set; } = null!;

    public DateOnly NgayDangKy { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
