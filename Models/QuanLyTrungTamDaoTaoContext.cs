using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTrungTamDaoTao.Models;

public partial class QuanLyTrungTamDaoTaoContext : DbContext
{
    public QuanLyTrungTamDaoTaoContext()
    {
    }

    public QuanLyTrungTamDaoTaoContext(DbContextOptions<QuanLyTrungTamDaoTaoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }

    public virtual DbSet<HocVien> HocViens { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DangKyKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaDangKy).HasName("PK__DangKyKh__BA90F02DE3350D1D");

            entity.ToTable("DangKyKhoaHoc");

            entity.Property(e => e.MaHocVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.DangKyKhoaHocs)
                .HasForeignKey(d => d.MaHocVien)
                .HasConstraintName("FK__DangKyKho__MaHoc__4D94879B");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.DangKyKhoaHocs)
                .HasForeignKey(d => d.MaKhoaHoc)
                .HasConstraintName("FK__DangKyKho__MaKho__4E88ABD4");
        });

        modelBuilder.Entity<HocVien>(entity =>
        {
            entity.HasKey(e => e.MaHocVien).HasName("PK__HocVien__685B0E6AADCDAD7F");

            entity.ToTable("HocVien");

            entity.Property(e => e.MaHocVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaKhoaHoc).HasName("PK__KhoaHoc__48F0FF9846C8EB35");

            entity.ToTable("KhoaHoc");

            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.GiangVien).HasMaxLength(255);
            entity.Property(e => e.HocPhi).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TenKhoaHoc).HasMaxLength(255);
        });

        modelBuilder.Entity<QuanTriVien>(entity =>
        {
            entity.HasKey(e => e.MaQuanTriVien).HasName("PK__tmp_ms_x__6112BB3BCF2CC037");

            entity.ToTable("QuanTriVien");

            entity.Property(e => e.MaQuanTriVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenQuanTriVien).HasMaxLength(255);
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C65291221368B");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenTaiKhoan, "UQ__TaiKhoan__B106EAF8A7B77E8A").IsUnique();

            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TenTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
