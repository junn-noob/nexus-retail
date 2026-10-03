using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NexusRetail.Shared.Entities;

public partial class RetailDbContext : DbContext
{
    public RetailDbContext(DbContextOptions<RetailDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatbotTuVanLog> ChatbotTuVanLogs { get; set; }

    public virtual DbSet<ChiTietHdb> ChiTietHdbs { get; set; }

    public virtual DbSet<ChiTietHdn> ChiTietHdns { get; set; }

    public virtual DbSet<HoaDonBan> HoaDonBans { get; set; }

    public virtual DbSet<HoaDonNhap> HoaDonNhaps { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<ManHinh> ManHinhs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhanHieu> NhanHieus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Que> Ques { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<VwChiTietHoaDonBan> VwChiTietHoaDonBans { get; set; }

    public virtual DbSet<VwDanhSachSanPham> VwDanhSachSanPhams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatbotTuVanLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("Chatbot_TuVanLog");

            entity.Property(e => e.MaSpGoiY)
                .HasMaxLength(20)
                .HasColumnName("MaSP_GoiY");
            entity.Property(e => e.NganSachToiDa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NhuCauKhach).HasMaxLength(500);
            entity.Property(e => e.SessionId).HasMaxLength(100);
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSpGoiYNavigation).WithMany(p => p.ChatbotTuVanLogs)
                .HasForeignKey(d => d.MaSpGoiY)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Chatbot_SanPham");
        });

        modelBuilder.Entity<ChiTietHdb>(entity =>
        {
            entity.HasKey(e => new { e.MaHdb, e.MaSp });

            entity.ToTable("ChiTietHDB", tb => tb.HasTrigger("trg_ChiTietHDB_Change"));

            entity.Property(e => e.MaHdb)
                .HasMaxLength(20)
                .HasColumnName("MaHDB");
            entity.Property(e => e.MaSp)
                .HasMaxLength(20)
                .HasColumnName("MaSP");
            entity.Property(e => e.KhuyenMai).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHdbNavigation).WithMany(p => p.ChiTietHdbs)
                .HasForeignKey(d => d.MaHdb)
                .HasConstraintName("FK_ChiTietHDB_HoaDonBan");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietHdbs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHDB_SanPham");
        });

        modelBuilder.Entity<ChiTietHdn>(entity =>
        {
            entity.HasKey(e => new { e.MaHdn, e.MaSp });

            entity.ToTable("ChiTietHDN", tb => tb.HasTrigger("trg_ChiTietHDN_Change"));

            entity.Property(e => e.MaHdn)
                .HasMaxLength(20)
                .HasColumnName("MaHDN");
            entity.Property(e => e.MaSp)
                .HasMaxLength(20)
                .HasColumnName("MaSP");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KhuyenMai).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHdnNavigation).WithMany(p => p.ChiTietHdns)
                .HasForeignKey(d => d.MaHdn)
                .HasConstraintName("FK_ChiTietHDN_HoaDonNhap");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietHdns)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHDN_SanPham");
        });

        modelBuilder.Entity<HoaDonBan>(entity =>
        {
            entity.HasKey(e => e.MaHdb);

            entity.ToTable("HoaDonBan");

            entity.Property(e => e.MaHdb)
                .HasMaxLength(20)
                .HasColumnName("MaHDB");
            entity.Property(e => e.MaKhach).HasMaxLength(20);
            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayBan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhachNavigation).WithMany(p => p.HoaDonBans)
                .HasForeignKey(d => d.MaKhach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonBan_KhachHang");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDonBans)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonBan_NhanVien");
        });

        modelBuilder.Entity<HoaDonNhap>(entity =>
        {
            entity.HasKey(e => e.MaHdn);

            entity.ToTable("HoaDonNhap");

            entity.Property(e => e.MaHdn)
                .HasMaxLength(20)
                .HasColumnName("MaHDN");
            entity.Property(e => e.MaNcc)
                .HasMaxLength(20)
                .HasColumnName("MaNCC");
            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayNhap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.HoaDonNhaps)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonNhap_NhaCungCap");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDonNhaps)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDonNhap_NhanVien");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhach);

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKhach).HasMaxLength(20);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.TenKhach).HasMaxLength(100);
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai);

            entity.ToTable("Loai");

            entity.Property(e => e.MaLoai).HasMaxLength(20);
            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<ManHinh>(entity =>
        {
            entity.HasKey(e => e.MaManHinh);

            entity.ToTable("ManHinh");

            entity.Property(e => e.MaManHinh).HasMaxLength(20);
            entity.Property(e => e.TenManHinh).HasMaxLength(100);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc);

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.MaNcc)
                .HasMaxLength(20)
                .HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.TenNcc)
                .HasMaxLength(150)
                .HasColumnName("TenNCC");
        });

        modelBuilder.Entity<NhanHieu>(entity =>
        {
            entity.HasKey(e => e.MaNhanHieu);

            entity.ToTable("NhanHieu");

            entity.Property(e => e.MaNhanHieu).HasMaxLength(20);
            entity.Property(e => e.TenNhanHieu).HasMaxLength(100);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv);

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.MaQue).HasMaxLength(20);
            entity.Property(e => e.TenNv)
                .HasMaxLength(100)
                .HasColumnName("TenNV");

            entity.HasOne(d => d.MaQueNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaQue)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_NhanVien_Que");
        });

        modelBuilder.Entity<Que>(entity =>
        {
            entity.HasKey(e => e.MaQue);

            entity.ToTable("Que");

            entity.Property(e => e.MaQue).HasMaxLength(20);
            entity.Property(e => e.TenQue).HasMaxLength(100);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp);

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSp)
                .HasMaxLength(20)
                .HasColumnName("MaSP");
            entity.Property(e => e.AmThanh).HasMaxLength(100);
            entity.Property(e => e.Anh).HasMaxLength(250);
            entity.Property(e => e.ChupAnh).HasMaxLength(200);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaLoai).HasMaxLength(20);
            entity.Property(e => e.MaManHinh).HasMaxLength(20);
            entity.Property(e => e.MaNhanHieu).HasMaxLength(20);
            entity.Property(e => e.TenSp)
                .HasMaxLength(150)
                .HasColumnName("TenSP");
            entity.Property(e => e.ThoiGianBaoHanh).HasDefaultValue(12);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_Loai");

            entity.HasOne(d => d.MaManHinhNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaManHinh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_ManHinh");

            entity.HasOne(d => d.MaNhanHieuNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNhanHieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_NhanHieu");
        });

        modelBuilder.Entity<VwChiTietHoaDonBan>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ChiTietHoaDonBan");

            entity.Property(e => e.DonGiaNiemYet).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KhuyenMai).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MaHdb)
                .HasMaxLength(20)
                .HasColumnName("MaHDB");
            entity.Property(e => e.MaKhach).HasMaxLength(20);
            entity.Property(e => e.MaNv)
                .HasMaxLength(20)
                .HasColumnName("MaNV");
            entity.Property(e => e.MaSp)
                .HasMaxLength(20)
                .HasColumnName("MaSP");
            entity.Property(e => e.NgayBan).HasColumnType("datetime");
            entity.Property(e => e.SdtKhach)
                .HasMaxLength(20)
                .HasColumnName("SDT_Khach");
            entity.Property(e => e.TenKhach).HasMaxLength(100);
            entity.Property(e => e.TenNv)
                .HasMaxLength(100)
                .HasColumnName("TenNV");
            entity.Property(e => e.TenSp)
                .HasMaxLength(150)
                .HasColumnName("TenSP");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTienHoaDon).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<VwDanhSachSanPham>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_DanhSachSanPham");

            entity.Property(e => e.AmThanh).HasMaxLength(100);
            entity.Property(e => e.Anh).HasMaxLength(250);
            entity.Property(e => e.ChupAnh).HasMaxLength(200);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaLoai).HasMaxLength(20);
            entity.Property(e => e.MaManHinh).HasMaxLength(20);
            entity.Property(e => e.MaNhanHieu).HasMaxLength(20);
            entity.Property(e => e.MaSp)
                .HasMaxLength(20)
                .HasColumnName("MaSP");
            entity.Property(e => e.TenLoai).HasMaxLength(100);
            entity.Property(e => e.TenManHinh).HasMaxLength(100);
            entity.Property(e => e.TenNhanHieu).HasMaxLength(100);
            entity.Property(e => e.TenSp)
                .HasMaxLength(150)
                .HasColumnName("TenSP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
