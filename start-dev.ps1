[CmdletBinding()]
param(
    [switch]$WithPos,
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$repoRoot = $PSScriptRoot
Push-Location $repoRoot

$spawned = New-Object System.Collections.Generic.List[System.Diagnostics.Process]

function Start-ServiceProcess {
    param(
        [string]$Name,
        [string]$ProjectPath,
        [string]$Url,
        [string]$EnvironmentName = 'Development'
    )

    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = 'dotnet'
    $psi.Arguments = "run --project `"$ProjectPath`" --no-build --no-launch-profile --urls `"$Url`""
    $psi.WorkingDirectory = $repoRoot
    $psi.UseShellExecute = $false
    $psi.Environment['ASPNETCORE_ENVIRONMENT'] = $EnvironmentName

    $proc = [System.Diagnostics.Process]::Start($psi)
    if ($null -eq $proc) {
        throw "Khong the khoi chay $Name."
    }

    $spawned.Add($proc)
    Write-Host "  [OK] $Name -> $Url (PID: $($proc.Id))" -ForegroundColor Green
}

try {
    if (-not $NoBuild) {
        Write-Host "[1/3] Dang bien dich giai phap NexusRetail.slnx..." -ForegroundColor Cyan
        dotnet build NexusRetail.slnx --verbosity minimal
        if ($LASTEXITCODE -ne 0) {
            throw "Bien dich that bai. Vui long kiem tra loi truoc khi khoi chay."
        }
    }

    Write-Host "[2/3] Dang khoi chay cac dich vu nen..." -ForegroundColor Cyan
    Start-ServiceProcess -Name 'Identity.Api' -ProjectPath 'src/NexusRetail.Identity.Api/NexusRetail.Identity.Api.csproj' -Url 'http://localhost:5001'
    Start-ServiceProcess -Name 'Core.Api'     -ProjectPath 'src/NexusRetail.Core.Api/NexusRetail.Core.Api.csproj'         -Url 'http://localhost:5050'
    Start-ServiceProcess -Name 'Gateway'      -ProjectPath 'src/NexusRetail.Gateway/NexusRetail.Gateway.csproj'           -Url 'http://localhost:5000'
    Start-ServiceProcess -Name 'WebMvc'       -ProjectPath 'src/NexusRetail.WebMvc/NexusRetail.WebMvc.csproj'             -Url 'http://localhost:5296'

    if ($WithPos) {
        Write-Host "[3/3] Dang mo ung dung WinForms POS..." -ForegroundColor Cyan
        Start-Process -FilePath 'dotnet' -ArgumentList 'run --project "src/NexusRetail.WinForms/NexusRetail.WinForms.csproj" --no-build' -WorkingDirectory $repoRoot | Out-Null
    } else {
        Write-Host "[3/3] Da khoi chay he thong Web & API (them -WithPos neu muon mo WinForms POS)." -ForegroundColor Cyan
    }

    Write-Host "`nCac dia chi truy cap:" -ForegroundColor White
    Write-Host "  - Web Storefront & Admin: http://localhost:5296"
    Write-Host "  - API Gateway (YARP):     http://localhost:5000"
    Write-Host "  - Core API (Scalar UI):   http://localhost:5050/scalar/v1"
    Write-Host "  - Identity API (Scalar):  http://localhost:5001/scalar/v1"
    Write-Host "`nNhan Ctrl+C de dung toan bo dich vu." -ForegroundColor Yellow

    while ($true) {
        Start-Sleep -Seconds 1
        foreach ($p in $spawned) {
            if ($p.HasExited) {
                Write-Warning "Tien trinh PID $($p.Id) da dung dot ngot voi exit code $($p.ExitCode)."
                return
            }
        }
    }
}
finally {
    Write-Host "`nDang dung cac dich vu..." -ForegroundColor Yellow
    foreach ($p in $spawned) {
        try {
            if (-not $p.HasExited) {
                Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
            }
        } catch {}
    }
    Pop-Location
}

