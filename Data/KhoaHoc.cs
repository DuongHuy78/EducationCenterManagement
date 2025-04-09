using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Data;

public partial class KhoaHoc
{
    public string MaKhoaHoc { get; set; } = null!;

    public string TenKhoaHoc { get; set; } = null!;

    public string? GiangVien { get; set; }

    public DateOnly? ThoiGianKhaiGiang { get; set; }

    public decimal? HocPhi { get; set; }

    public int? SoLuongHocVienToiDa { get; set; }

    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();
}
