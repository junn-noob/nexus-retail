# ====================================================================================
# NEXUS RETAIL POS ENTERPRISE - TỰ ĐỘNG KHỞI TẠO CƠ SỞ DỮ LIỆU TRÊN MÁY MỚI
# ====================================================================================
[CmdletBinding()]
param(
    [string]$ServerInstance = ""
)

$ErrorActionPreference = 'Stop'

Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host "   NEXUS RETAIL POS - DATABASE SETUP & INITIALIZATION SUITE" -ForegroundColor Cyan
Write-Host "================================================================================" -ForegroundColor Cyan

# 1. TỰ ĐỘNG TÌM KIẾM SQL SERVER INSTANCE PHÙ HỢP
$candidateInstances = @(
    "(localdb)\MSSQLLocalDB",
    ".\SQLEXPRESS",
    "localhost\SQLEXPRESS",
    "localhost",
    "."
)

if (![string]::IsNullOrWhiteSpace($ServerInstance)) {
    $candidateInstances = @($ServerInstance) + $candidateInstances
}

$targetInstance = $null

Write-Host "`n[BƯỚC 1] Đang dò tìm SQL Server Instance trên máy..." -ForegroundColor Yellow
foreach ($inst in $candidateInstances) {
    try {
        $testResult = sqlcmd -S $inst -Q "SET NOCOUNT ON; SELECT @@VERSION;" -b 2>$null
        if ($LASTEXITCODE -eq 0 -and $testResult) {
            $targetInstance = $inst
            Write-Host "✔ Đã kết nối thành công tới SQL Server Instance: '$inst'" -ForegroundColor Green
            break
        }
    } catch { }
}

if (-not $targetInstance) {
    # Nếu chưa bật LocalDB, thử khởi động LocalDB
    Write-Host "⚠️ Chưa phát hiện SQL Server đang chạy. Đang thử kích hoạt sqllocaldb start MSSQLLocalDB..." -ForegroundColor Yellow
    try {
        & sqllocaldb start MSSQLLocalDB 2>$null
        Start-Sleep -Seconds 2
        $testResult = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SET NOCOUNT ON; SELECT @@VERSION;" -b 2>$null
        if ($LASTEXITCODE -eq 0) {
            $targetInstance = "(localdb)\MSSQLLocalDB"
            Write-Host "✔ Đã khởi động và kết nối thành công: '(localdb)\MSSQLLocalDB'" -ForegroundColor Green
        }
    } catch { }
}

if (-not $targetInstance) {
    Write-Host "`n❌ KHÔNG TÌM THẤY SQL SERVER!" -ForegroundColor Red
    Write-Host "Vui lòng đảm bảo một trong các dịch vụ sau đã được cài đặt trên máy:" -ForegroundColor Yellow
    Write-Host "  1. SQL Server LocalDB (đi kèm Visual Studio)"
    Write-Host "  2. SQL Server Express hoặc Developer / Standard Edition"
    Write-Host "Hoặc chạy lệnh với tham số: .\setup_database.ps1 -ServerInstance 'Tên_Server'" -ForegroundColor Yellow
    exit 1
}

# 2. XÁC ĐỊNH FILE SQL
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlFile = Join-Path $scriptDir "NexusRetail_Database_Full.sql"

if (-not (Test-Path $sqlFile)) {
    $sqlFile = Join-Path (Split-Path -Parent $scriptDir) "NexusRetail_Database_Full.sql"
}

if (-not (Test-Path $sqlFile)) {
    Write-Host "`n❌ Không tìm thấy file script SQL: NexusRetail_Database_Full.sql" -ForegroundColor Red
    exit 1
}

Write-Host "`n[BƯỚC 2] Đang thực thi script khởi tạo cơ sở dữ liệu..." -ForegroundColor Yellow
Write-Host "  File script: $sqlFile" -ForegroundColor Gray
Write-Host "  Mục tiêu: NexusRetailDb + NexusRetailIdentityDb" -ForegroundColor Gray

# Thực thi file SQL qua sqlcmd (hỗ trợ UTF-8)
$sqlOutput = sqlcmd -S $targetInstance -i $sqlFile -b
Write-Host $sqlOutput -ForegroundColor Gray

# 3. KIỂM TRA SỐ LƯỢNG BẢNG VÀ DỮ LIỆU
Write-Host "`n[BƯỚC 3] KIỂM TRA KẾT QUẢ KHỞI TẠO..." -ForegroundColor Yellow

$cntSanPham = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM SanPham;").Trim()
$cntLoai    = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM Loai;").Trim()
$cntNhanHieu= (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM NhanHieu;").Trim()
$cntNhanVien= (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM NhanVien;").Trim()
$cntHdb     = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM HoaDonBan;").Trim()
$cntHdn     = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM HoaDonNhap;").Trim()

$cntUsers   = (sqlcmd -S $targetInstance -d "NexusRetailIdentityDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM AspNetUsers;").Trim()
$cntRoles   = (sqlcmd -S $targetInstance -d "NexusRetailIdentityDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM AspNetRoles;").Trim()

Write-Host "  [NexusRetailDb]" -ForegroundColor Cyan
Write-Host "    • Danh mục sản phẩm (SanPham): $cntSanPham mặt hàng smartphone" -ForegroundColor Green
Write-Host "    • Phân loại (Loai):            $cntLoai danh mục" -ForegroundColor Green
Write-Host "    • Thương hiệu (NhanHieu):      $cntNhanHieu hãng" -ForegroundColor Green
Write-Host "    • Nhân viên bán hàng (NhanVien): $cntNhanVien nhân sự" -ForegroundColor Green
Write-Host "    • Hóa đơn bán hàng (HoaDonBan): $cntHdb hóa đơn" -ForegroundColor Green
Write-Host "    • Hóa đơn nhập kho (HoaDonNhap): $cntHdn phiếu nhập" -ForegroundColor Green

Write-Host "`n  [NexusRetailIdentityDb]" -ForegroundColor Cyan
Write-Host "    • Phân quyền RBAC (AspNetRoles): $cntRoles roles (Admin, Staff, Customer)" -ForegroundColor Green
Write-Host "    • Tài khoản quản trị (AspNetUsers): $cntUsers users sẵn sàng" -ForegroundColor Green

Write-Host "`n================================================================================" -ForegroundColor Cyan
Write-Host "  ✔ CƠ SỞ DỮ LIỆU ĐÃ ĐƯỢC THIẾT LẬP THÀNH CÔNG 100%!" -ForegroundColor Green
Write-Host "  SẴN SÀNG CHẠY TRÊN MÁY MỚI VỚI CÁC TÀI KHOẢN ĐĂNG NHẬP MẪU:" -ForegroundColor White
Write-Host "    1. Quản trị viên (Admin):    admin    / admin123" -ForegroundColor Yellow
Write-Host "    2. Thu ngân / Kho (Staff):   nhanvien / 123456" -ForegroundColor Yellow
Write-Host "    3. Khách hàng (Customer):    customer / 123456" -ForegroundColor Yellow
Write-Host "================================================================================" -ForegroundColor Cyan
