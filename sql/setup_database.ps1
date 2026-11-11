# ====================================================================================
# NEXUS RETAIL POS ENTERPRISE - TU DONG KHOI TAO CO SO DU LIEU TREN MAY MOI
# ====================================================================================
[CmdletBinding()]
param(
    [string]$ServerInstance = ""
)

$ErrorActionPreference = 'Stop'
$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host "   NEXUS RETAIL POS - DATABASE SETUP & INITIALIZATION SUITE" -ForegroundColor Cyan
Write-Host "================================================================================" -ForegroundColor Cyan

# 1. TU DONG TIM KIEM SQL SERVER INSTANCE PHU HOP
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

Write-Host ""
Write-Host "[BUOC 1] Dang do tim SQL Server Instance tren may..." -ForegroundColor Yellow
foreach ($inst in $candidateInstances) {
    try {
        $testResult = sqlcmd -S $inst -Q "SET NOCOUNT ON; SELECT @@VERSION;" -b 2>$null
        if ($LASTEXITCODE -eq 0 -and $testResult) {
            $targetInstance = $inst
            Write-Host "[OK] Da ket noi thanh cong toi SQL Server: '$inst'" -ForegroundColor Green
            break
        }
    } catch {
        # Bo qua neu instance chua bat hoac khong ton tai
    }
}

if (-not $targetInstance) {
    # Neu chua bat LocalDB, thu khoi dong LocalDB
    Write-Host "[INFO] Chua phat hien SQL Server dang chay. Dang thu kich hoat MSSQLLocalDB..." -ForegroundColor Yellow
    try {
        & sqllocaldb start MSSQLLocalDB 2>$null
        Start-Sleep -Seconds 2
        $testResult = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SET NOCOUNT ON; SELECT @@VERSION;" -b 2>$null
        if ($LASTEXITCODE -eq 0) {
            $targetInstance = "(localdb)\MSSQLLocalDB"
            Write-Host "[OK] Da khoi dong va ket noi thanh cong: '(localdb)\MSSQLLocalDB'" -ForegroundColor Green
        }
    } catch {
        # Bo qua
    }
}

if (-not $targetInstance) {
    Write-Host ""
    Write-Host "[ERROR] KHONG TIM THAY SQL SERVER DANG CHAY!" -ForegroundColor Red
    Write-Host "Vui long dam bao mot trong cac dich vu sau da duoc cai dat tren may:" -ForegroundColor Yellow
    Write-Host "  1. SQL Server LocalDB (di kem Visual Studio)"
    Write-Host "  2. SQL Server Express hoac Developer / Standard Edition"
    Write-Host "Hoac chay lenh voi tham so: .\setup_database.ps1 -ServerInstance 'Ten_Server'" -ForegroundColor Yellow
    exit 1
}

# 2. XAC DINH FILE SQL
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlFile = Join-Path $scriptDir "NexusRetail_Database_Full.sql"

if (-not (Test-Path $sqlFile)) {
    $sqlFile = Join-Path (Split-Path -Parent $scriptDir) "NexusRetail_Database_Full.sql"
}

if (-not (Test-Path $sqlFile)) {
    Write-Host ""
    Write-Host "[ERROR] Khong tim thay file script SQL: NexusRetail_Database_Full.sql" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[BUOC 2] Dang thuc thi script khoi tao co so du lieu..." -ForegroundColor Yellow
Write-Host "  File script: $sqlFile" -ForegroundColor Gray
Write-Host "  Muc tieu: NexusRetailDb + NexusRetailIdentityDb" -ForegroundColor Gray

# Thuc thi file SQL qua sqlcmd (UTF-8 codepage 65001)
$sqlOutput = sqlcmd -S $targetInstance -f 65001 -i $sqlFile -b
Write-Host $sqlOutput -ForegroundColor Gray

# 3. KIEM TRA SO LUONG BANG VA DU LIEU
Write-Host ""
Write-Host "[BUOC 3] KIEM TRA KET QUA KHOI TAO..." -ForegroundColor Yellow

$cntSanPham = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM SanPham;").Trim()
$cntLoai    = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM Loai;").Trim()
$cntNhanHieu= (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM NhanHieu;").Trim()
$cntNhanVien= (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM NhanVien;").Trim()
$cntHdb     = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM HoaDonBan;").Trim()
$cntHdn     = (sqlcmd -S $targetInstance -d "NexusRetailDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM HoaDonNhap;").Trim()

$cntUsers   = (sqlcmd -S $targetInstance -d "NexusRetailIdentityDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM AspNetUsers;").Trim()
$cntRoles   = (sqlcmd -S $targetInstance -d "NexusRetailIdentityDb" -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM AspNetRoles;").Trim()

Write-Host "  [NexusRetailDb]" -ForegroundColor Cyan
Write-Host "    - Danh muc san pham (SanPham):     $cntSanPham mat hang smartphone" -ForegroundColor Green
Write-Host "    - Phan loai (Loai):                $cntLoai danh muc" -ForegroundColor Green
Write-Host "    - Thuong hieu (NhanHieu):          $cntNhanHieu hang" -ForegroundColor Green
Write-Host "    - Nhan vien ban hang (NhanVien):   $cntNhanVien nhan su" -ForegroundColor Green
Write-Host "    - Hoa don ban hang (HoaDonBan):    $cntHdb hoa don" -ForegroundColor Green
Write-Host "    - Hoa don nhap kho (HoaDonNhap):   $cntHdn phieu nhap" -ForegroundColor Green

Write-Host ""
Write-Host "  [NexusRetailIdentityDb]" -ForegroundColor Cyan
Write-Host "    - Phan quyen RBAC (AspNetRoles):   $cntRoles roles (Admin, Staff, Customer)" -ForegroundColor Green
Write-Host "    - Tai khoan quan tri (AspNetUsers): $cntUsers users san sang" -ForegroundColor Green

Write-Host ""
Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host "  [SUCCESS] CO SO DU LIEU DA DUOC THIET LAP THANH CONG 100%!" -ForegroundColor Green
Write-Host "  SAN SANG CHAY TREN MAY MOI VOI CAC TAI KHOAN DANG NHAP MAU:" -ForegroundColor White
Write-Host "    1. Quan tri vien (Admin):    admin     / admin123" -ForegroundColor Yellow
Write-Host "    2. Thu ngan / Kho (Staff):   nhanvien  / 123456" -ForegroundColor Yellow
Write-Host "    3. Khach hang (Customer):    khachhang / 123456" -ForegroundColor Yellow
Write-Host "================================================================================" -ForegroundColor Cyan
