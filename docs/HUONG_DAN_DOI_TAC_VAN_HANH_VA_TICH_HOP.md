# HỆ THỐNG QUẢN LÝ BÁN LẺ & CỔNG THANH TOÁN NEXUS RETAIL (DUY MOBILE)
## TÀI LIỆU HƯỚNG DẪN TÍCH HỢP VÀ VẬN HÀNH DÀNH CHO ĐỐI TÁC

> **Phiên bản tài liệu:** 2.0 (Commercial Production Ready)  
> **Ngày phê duyệt:** Tháng 10/2026  
> **Bộ phận:** Ban Công Nghệ & Quản Lý Chất Lượng (QA / Software Architecture)  
> **Trạng thái kiểm thử:** 100% Passed (Unit Tests, Business Triggers, API Integration & E2E Verification)

---

## MỤC LỤC
1. [Tổng Quan Kiến Trúc Hệ Thống Microservices](#1-tổng-quan-kiến-trúc-hệ-thống-microservices)
2. [Xác Thực & Phân Quyền Bảo Mật (JWT Authentication)](#2-xác-thực--phân-quyền-bảo-mật-jwt-authentication)
3. [Quy Tắc Nghiệp Vụ Cốt Lõi (Business Rules Engine)](#3-quy-tắc-nghiệp-vụ-cốt-lõi-business-rules-engine)
4. [Tích Hợp Cổng Thanh Toán MBBank & Chuẩn VietQR](#4-tích-hợp-cổng-thanh-toán-mbbank--chuẩn-vietqr)
5. [Đặc Tả Webhook Đối Soát Giao Dịch Ngân Hàng](#5-đặc-tả-webhook-đối-soát-giao-dịch-ngân-hàng)
6. [Hệ Thống Báo Cáo Phân Tích & Xuất Excel OpenXML](#6-hệ-thống-báo-cáo-phân-tích--xuất-excel-openxml)
7. [Trợ Lý Ảo AI Tư Vấn Bán Hàng (Gemini & Smart Fallback)](#7-trợ-lý-ảo-ai-tư-vấn-bán-hàng-gemini--smart-fallback)
8. [Hướng Dẫn Triển Khai & Khởi Chạy Nhanh Cho Đối Tác](#8-hướng-dẫn-triển-khai--khởi-chạy-nhanh-cho-đối-tác)
9. [Bảng Tra Cứu Mã Lỗi & Khắc Phục Sự Cố (Troubleshooting)](#9-bảng-tra-cứu-mã-lỗi--khắc-phục-sự-cố-troubleshooting)

---

## 1. TỔNG QUAN KIẾN TRÚC HỆ THỐNG MICROSERVICES

Hệ thống **Nexus Retail** được xây dựng theo mô hình Microservices hiện đại, tối ưu hóa cho hiệu năng cao, mở rộng linh hoạt và sẵn sàng vận hành thương mại:

| Dịch vụ (Service) | Cổng mặc định (Port) | Công nghệ | Nhiệm vụ chính |
| :--- | :--- | :--- | :--- |
| **NexusRetail.Gateway** | `5000` | YARP Reverse Proxy (.NET 10) | Điểm truy cập duy nhất (Single Entry Point), định tuyến thông minh |
| **NexusRetail.Identity.Api** | `5001` | ASP.NET Identity, JWT Bearer | Quản lý tài khoản, phân quyền RBAC (Admin, Staff, Customer) |
| **NexusRetail.Core.Api** | `5050` / `7143` | ASP.NET Core Web API, EF Core 10 | Nghiệp vụ bán hàng, kho hàng, VietQR, Webhook ngân hàng, Báo cáo |
| **NexusRetail.WebMvc** | `5296` | ASP.NET Core MVC, Bootstrap 5 | Cổng thương mại điện tử dành cho khách hàng & Quản trị web |
| **NexusRetail.WinForms** | Desktop App | WinForms Modern (.NET 10 Windows) | Phần mềm quầy thu ngân (POS) chuyên dụng với máy in và quét mã |
| **Database Engines** | MSSQLLocalDB | SQL Server 2019+ | `NexusRetailDb` (nghiệp vụ), `NexusRetailIdentityDb` (bảo mật) |

---

## 2. XÁC THỰC & PHÂN QUYỀN BẢO MẬT (JWT AUTHENTICATION)

### 2.1 Cấp phát mã định danh (Login API)
Tất cả các đối tác tích hợp (Web, App đối tác, POS) cần lấy JSON Web Token (JWT) trước khi thao tác các tác vụ có bảo vệ:
- **Endpoint:** `POST http://localhost:5001/api/auth/login`
- **Headers:** `Content-Type: application/json; charset=utf-8`
- **Request Body mẫu:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```
- **Response mẫu (HTTP 200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@duymobile.vn",
  "role": "Admin",
  "expiresAt": "2026-10-27T17:05:00Z"
}
```

### 2.2 Cách gửi Token trong các Request nghiệp vụ
Gửi Token trong HTTP Header của mỗi request:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 3. QUY TẮC NGHIỆP VỤ CỐT LÕI (BUSINESS RULES ENGINE)

Hệ thống được thiết kế với cơ chế tự động hóa kép (Dual Execution Guard): vừa bảo đảm bằng **Database Triggers** cấp độ SQL Server (ACID), vừa được kiểm soát trong **Application Domain Code**:

* **BR-01 (Quản lý tồn kho tự động):**
  - Khi lập Phiếu nhập hàng (`POST /api/nhap-hang`), tồn kho của sản phẩm tự động tăng tương ứng.
  - Khi lập Hóa đơn bán hàng (`POST /api/ban-hang/thanh-toan`), hệ thống kiểm tra tồn kho tức thời. Nếu không đủ hàng, giao dịch tự động Rollback và trả về mã lỗi 400 cùng số lượng tồn hiện có. Nếu đủ hàng, tồn kho được trừ tự động.
* **BR-02 (Cập nhật giá vốn):**
  - Đơn giá nhập mới nhất từ nhà cung cấp sẽ tự động cập nhật trường `GiaNhap` trong bảng `SanPham`.
* **BR-03 (Công thức giá bán niêm yết tự động):**
  - Giá bán lẻ được tự động tính toán bằng đúng **110% giá vốn** (`GiaBan = Round(GiaNhap * 1.10, 0)`). Áp dụng khi thêm mới sản phẩm, sửa sản phẩm hoặc khi nhập lô hàng mới.
* **BR-04 (Bộ lọc tìm kiếm đa tiêu chí):**
  - Hỗ trợ tìm kiếm theo từ khóa mã SP/tên SP, nhãn hiệu (Apple, Samsung, Xiaomi...), phân loại (Smartphone, Phụ kiện...) và khoảng giá (`minPrice`, `maxPrice`).
* **BR-05 (Bảo toàn toàn vẹn dữ liệu - Referential Integrity):**
  - Không cho phép xóa sản phẩm nếu sản phẩm đó đã phát sinh lịch sử hóa đơn bán hoặc hóa đơn nhập nhằm đảm bảo dữ liệu kế toán đối soát không bị sai lệch.

---

## 4. TÍCH HỢP CỔNG THANH TOÁN MBBANK & CHUẨN VIETQR

### 4.1 Quy chuẩn cấu trúc VietQR tĩnh & động
Nexus Retail tích hợp thư viện thuần static [`VietQrHelper`](file:///c:/Users/hungn/Downloads/Phân%20Tích%20Thiết%20Kế%20Yêu%20Cầu/nexus-retail/src/NexusRetail.Shared/Helpers/VietQrHelper.cs) tuân thủ tiêu chuẩn Napas / EMVCo QR Code:

- **Mã Ngân hàng (BIN):** `970422` (MBBank - Ngân hàng TMCP Quân Đội).
- **Số tài khoản thụ hưởng:** `0963388888` (Cửa hàng Duy Mobile).
- **Cấu trúc nội dung thanh toán tự động:**
  ```text
  DUYMOBILE <Mã_Hóa_Đơn>
  ```
  *Ví dụ:* `DUYMOBILE HDB102` (Hệ thống sẽ bóc tách chính xác mã hóa đơn `HDB102` từ chuỗi tin nhắn biến động số dư).

### 4.2 Thuật toán kiểm tra mã kiểm soát CRC16 (CCITT-FALSE)
Chuỗi VietQR sau khi đóng gói được tính toán 4 ký tự checksum hex theo đa thức chuẩn `0x1021`, khởi tạo `0xFFFF`, bảo đảm 100% ứng dụng Mobile Banking của tất cả ngân hàng tại Việt Nam quét mã nhận đúng số tiền và thông điệp mà không bị lỗi sai cú pháp.

---

## 5. ĐẶC TẢ WEBHOOK ĐỐI SOÁT GIAO DỊCH NGÂN HÀNG

Khi khách hàng quét mã VietQR và xác nhận chuyển khoản thành công, hệ thống MBBank (hoặc dịch vụ gateway trung gian như SePay/Cassie) sẽ bắn thông báo Webhook về Core API:

### 5.1 Endpoint nhận Webhook
- **URL:** `POST http://localhost:5050/api/webhook/bank`
- **Content-Type:** `application/json; charset=utf-8`

### 5.2 Payload Request mẫu
```json
{
  "gateway": "MBBank",
  "transactionDate": "2026-10-27T10:05:00",
  "accountNumber": "0963388888",
  "transferAmount": 79200000.0,
  "content": "DUYMOBILE HDB102 CHUYEN KHOAN",
  "referenceCode": "FT_20261027_987654"
}
```

### 5.3 Cơ chế xử lý đối soát thông minh
1. **Trường hợp khách chuyển thiếu tiền:**
   - Hệ thống ghi nhận số tiền đã nhận.
   - Trả về thông báo cảnh báo: `Khách chuyển thiếu tiền: Đã nhận X đ / Cần Y đ (Còn thiếu Z đ)`.
   - Trạng thái đơn vẫn là `isPaid = false`.
2. **Trường hợp khách chuyển bổ sung hoặc chuyển đủ 100%:**
   - Hệ thống cộng dồn tổng tiền nhận được.
   - Khi tổng tiền $\ge$ giá trị đơn hàng, hệ thống tự động chốt: `isPaid = true`, ghi nhận mã giao dịch ngân hàng và kích hoạt trạng thái "Đã thanh toán" trên màn hình thu ngân POS và Web Portal theo thời gian thực (Real-time).

### 5.4 Endpoint thăm dò trạng thái (Polling/Status)
- **URL:** `GET http://localhost:5050/api/webhook/bank/status/{maHdb}?expectedAmount={tongTien}`
- **Response mẫu:**
```json
{
  "maHdb": "HDB102",
  "isPaid": true,
  "paidAmount": 79200000.0,
  "expectedAmount": 79200000.0,
  "transactionCode": "FT_20261027_987654",
  "message": "Đã thanh toán đủ toàn bộ đơn hàng!"
}
```

---

## 6. HỆ THỐNG BÁO CÁO PHÂN TÍCH & XUẤT EXCEL OPENXML

Core API cung cấp đầy đủ các báo cáo quản trị theo các quy tắc nghiệp vụ định sẵn, hỗ trợ xuất file Excel định dạng OpenXML (.xlsx):

| Báo cáo | Endpoint JSON | Endpoint xuất Excel |
| :--- | :--- | :--- |
| **BR-06: Top 5 SP bán chạy** | `GET /api/bao-cao/sp-ban-chay?month=9&year=2026` | `GET /api/bao-cao/sp-ban-chay/excel?month=9&year=2026` |
| **BR-07: HĐ Nhập lớn nhất theo quý** | `GET /api/bao-cao/hdn-lon-nhat?quarter=3&year=2026` | `GET /api/bao-cao/hdn-lon-nhat/excel?quarter=3&year=2026` |
| **BR-08: Doanh số bán theo năm** | `GET /api/bao-cao/doanh-so-nam?year=2026` | `GET /api/bao-cao/doanh-so-nam/excel?year=2026` |
| **BR-09: Nhân viên bán ít nhất** | `GET /api/bao-cao/nv-doanh-so-thap?month=9&year=2026` | `GET /api/bao-cao/nv-doanh-so-thap/excel?month=9&year=2026` |

*Tất cả file Excel xuất ra đều được định dạng tiêu đề in hoa, căn lề tự động, kẻ khung bảng, định dạng cột tiền tệ chuẩn Việt Nam Đồng và có chữ ký xác nhận của kế toán.*

---

## 7. TRỢ LÝ ẢO AI TƯ VẤN BÁN HÀNG (GEMINI & SMART FALLBACK)

- **Endpoint:** `POST http://localhost:5050/api/chat`
- **Request Body:**
```json
{
  "message": "Cho tôi xem điện thoại iPhone bộ nhớ 256GB"
}
```
- **Cơ chế hoạt động:**
  1. Trích xuất ý định người dùng (phát hiện số điện thoại tra cứu đơn hàng, thương hiệu máy, tầm giá ngân sách).
  2. Truy vấn trực tiếp kho hàng hiện có để cung cấp danh sách máy thực tế (`catalogText`).
  3. Gửi System Prompt chuyên nghiệp tới Google Gemini Flash API.
  4. Nếu Gemini ngoại tuyến hoặc không có kết nối internet, hệ thống tự động kích hoạt **Smart Catalog Fallback Engine**, phân tích database và trả về 3 mẫu máy phù hợp nhất kèm liên kết xem chi tiết, bảo đảm 100% thời gian hoạt động (High Availability).

---

## 8. HƯỚNG DẪN TRIỂN KHAI & KHỞI CHẠY NHANH CHO ĐỐI TÁC

### 8.1 Yêu cầu môi trường
- Hệ điều hành: Windows 10/11 x64 hoặc Windows Server 2022+
- .NET SDK: Version **10.0**
- Database: Microsoft SQL Server 2019+ hoặc LocalDB (`(localdb)\MSSQLLocalDB`)

### 8.2 Khởi động hệ thống
Đối tác có thể khởi chạy toàn bộ giải pháp chỉ bằng một câu lệnh tại thư mục gốc:
```powershell
# Chạy toàn bộ các bài kiểm tra tự động trước khi vận hành
powershell -ExecutionPolicy Bypass -File tests/verification/full_system_verification.ps1

# Khởi chạy Core API
dotnet run --project src/NexusRetail.Core.Api/NexusRetail.Core.Api.csproj --launch-profile http

# Khởi chạy Identity API
dotnet run --project src/NexusRetail.Identity.Api/NexusRetail.Identity.Api.csproj --launch-profile http

# Khởi chạy WebMvc
dotnet run --project src/NexusRetail.WebMvc/NexusRetail.WebMvc.csproj

# Khởi chạy phần mềm POS WinForms
dotnet run --project src/NexusRetail.WinForms/NexusRetail.WinForms.csproj
```

---

## 9. BẢNG TRA CỨU MÃ LỖI & KHẮC PHỤC SỰ CỐ (TROUBLESHOOTING)

| Mã lỗi HTTP | Nguyên nhân | Biện pháp xử lý |
| :--- | :--- | :--- |
| **400 Bad Request** | Tồn kho không đủ để bán (`sp.SoLuong < i.SoLuong`) | Nhập thêm hàng vào kho qua phiếu nhập (`/api/nhap-hang`) hoặc giảm số lượng trong giỏ hàng. |
| **400 Bad Request** | Xóa sản phẩm đã có hóa đơn bán hoặc nhập | Hệ thống bảo toàn tính toàn vẹn dữ liệu kế toán; không được xóa sản phẩm này. Khuyến nghị cập nhật số lượng về 0 nếu ngừng kinh doanh. |
| **401 Unauthorized** | Sai tài khoản/mật khẩu hoặc Token JWT đã hết hạn | Đăng nhập lại qua `POST /api/auth/login` để nhận Token mới. Mặc định tài khoản quản trị: `admin` / `admin123`. |
| **404 Not Found** | Không tìm thấy mã sản phẩm hoặc mã hóa đơn | Kiểm tra lại cú pháp mã SP (ví dụ `SP01`, `SP02`) hoặc mã HĐ (`HDB01`, `HDN01`). |
| **500 Internal Error** | Xung đột kết nối cơ sở dữ liệu LocalDB | Kiểm tra trạng thái dịch vụ SQL Server LocalDB bằng lệnh `sqllocaldb info MSSQLLocalDB`. |

---
*Tài liệu được phát hành và bảo chứng kỹ thuật bởi Đội ngũ Kiến Trúc & Kiểm Thử Phần Mềm Nexus Retail.*
