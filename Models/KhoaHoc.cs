using System;
using System.Collections.Generic;

namespace QuanLyTrungTamDaoTao.Models;

public partial class KhoaHoc
{
    public string MaKhoaHoc { get; set; } = null!;

    public string TenKhoaHoc { get; set; } = null!;

    public string GiangVien { get; set; } = null!;

    public DateOnly ThoiGianKhaiGiang { get; set; }

    public DateOnly? ThoiGianKetThuc { get; set; }

    public decimal HocPhi { get; set; }

    public int SoLuongHocVienToiDa { get; set; }

    public int SoLuongHocVienHienTai { get; set; } = 0;

    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();
}
