# Nexus Retail (Duy Mobile) — Hệ Thống Bán Lẻ Thiết Bị Di Động

Giải pháp quản lý bán hàng đa nền tảng trên **.NET 10.0**, kết hợp **WinForms POS**, **ASP.NET Core MVC Web Storefront & Admin Portal**, **YARP API Gateway**, **Identity & RBAC JWT Service**, và **Core REST API** theo kiến trúc 3 tầng (**Controller — BUS — DAL/EF Core + ADO.NET**).

---

## Cấu trúc dự án

```text
nexus-retail/
├── Directory.Build.props
├── NexusRetail.slnx
├── README.md
├── start-dev.ps1                          # Khởi chạy nhanh toàn bộ Web/API (+ -WithPos cho WinForms)
├── docs/
│   └── HUONG_DAN_DOI_TAC_VAN_HANH_VA_TICH_HOP.md
├── sql/
│   ├── NexusRetail_Database_Full.sql      # Schema, Views, Stored Procedures & Seed dữ liệu chuẩn
│   └── setup_database.ps1                 # Tự động dò SQL Server & khởi tạo CSDL
├── src/
│   ├── NexusRetail.Shared/                # Entities (ChungTuBase, HoaDonBan...), DTOs, VietQrHelper
│   ├── NexusRetail.Identity.Api/          # ASP.NET Core Identity + JWT + Role RBAC (Port 5001)
│   ├── NexusRetail.Core.Api/              # Core REST API (3-tier: Controllers / BUS / Data + EF Migrations)
│   ├── NexusRetail.Gateway/               # YARP Reverse Proxy định tuyến tập trung (Port 5000)
│   ├── NexusRetail.WebMvc/                # Website TMĐT & Cổng quản trị Admin/Staff (Port 5296 / 7122)
│   ├── NexusRetail.WinForms/              # Ứng dụng POS WinForms & Báo cáo (PrintDocument / HTML)
│   └── NexusRetail.UnitTests/             # Bộ kiểm thử đơn vị xUnit
└── tests/
    └── verification/
        └── full_system_verification.ps1   # Kịch bản kiểm thử luồng nghiệp vụ End-to-End
```

---

## Kiến trúc dịch vụ & Cổng mặc định (`Development`)

| Dịch vụ | HTTP | HTTPS | Vai trò |
| :--- | :--- | :--- | :--- |
| `NexusRetail.Gateway` | `http://localhost:5000` | — | YARP Reverse Proxy (`/api/auth/*` -> 5001, `/api/*` -> 5050) |
| `NexusRetail.Identity.Api` | `http://localhost:5001` | — | Xác thực JWT & Phân quyền (`Admin`, `Staff`, `Customer`) |
| `NexusRetail.Core.Api` | `http://localhost:5050` | `https://localhost:7143` | Danh mục, Bán hàng, Nhập kho, Báo cáo (`sp_BaoCao_*`), Webhook VietQR |
| `NexusRetail.WebMvc` | `http://localhost:5296` | `https://localhost:7122` | Storefront TMĐT, Giỏ hàng, Cổng quản trị Admin/Staff |
| `NexusRetail.WinForms` | Desktop | — | POS bán hàng, Quản lý sản phẩm/kho, In báo cáo A4 & Xuất Excel |

---

## Cài đặt & Khởi chạy nhanh

### 1. Khởi tạo cơ sở dữ liệu (SQL Server / LocalDB)

```powershell
.\sql\setup_database.ps1
```

Hoặc sử dụng EF Core Migrations (chế độ `Development`):

```powershell
dotnet ef database update --project src/NexusRetail.Core.Api -- --environment Development
dotnet ef database update --project src/NexusRetail.Identity.Api -- --environment Development
```

> Lưu ý cấu hình bí mật cục bộ (không commit lên Git):
> - Trong `Development`, hệ thống chạy sẵn với SQL Server LocalDB và cấu hình mẫu trong `appsettings.Development.json`.
> - Để cấu hình chuỗi kết nối SQL Server riêng, `Gemini:ApiKey`, `Webhook:Secret` hoặc `Jwt:Key` cho `Production` (>= 32 ký tự), hãy sao chép mẫu `appsettings.Local.example.json` thành `appsettings.Local.json` trong `src/NexusRetail.Core.Api` và `src/NexusRetail.Identity.Api` (hoặc đặt biến môi trường `NEXUS_JWT_SECRET`, `GEMINI_API_KEY`).

### 2. Khởi chạy toàn bộ hệ thống (1 lệnh)

```powershell
# Khởi chạy Identity.Api (5001), Core.Api (5050), Gateway (5000) và WebMvc (5296)
.\start-dev.ps1

# Hoặc khởi chạy kèm ứng dụng Desktop POS WinForms
.\start-dev.ps1 -WithPos
```

### 3. Biên dịch & Chạy kiểm thử tự động

```powershell
dotnet build NexusRetail.slnx
dotnet test NexusRetail.slnx
```

### 4. Tài khoản mẫu (`Identity.Api` seeding trong DB)

| Vai trò | Tên đăng nhập | Mật khẩu |
| :--- | :--- | :--- |
| Quản trị (`Admin`) | `admin` | `admin123` |
| Nhân viên (`Staff`) | `nhanvien` | `123456` |
| Khách hàng (`Customer`) | `khachhang` | `123456` |

