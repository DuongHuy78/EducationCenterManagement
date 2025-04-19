using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Models;

public partial class DangKyKhoaHoc
{
    public int MaDangKy { get; set; }

    public string? MaHocVien { get; set; }

    public string? MaKhoaHoc { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public DateOnly? NgayHuyDangKy { get; set; }

    public virtual HocVien? MaHocVienNavigation { get; set; }

    public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }
}
