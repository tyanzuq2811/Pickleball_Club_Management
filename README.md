# HỆ THỐNG QUẢN LÝ CLB PICKLEBALL "VỢT THỦ PHỐ NÚI" (PCM) - PRO EDITION

**Bài Kiểm Tra 02 - Phiên bản Nâng cao (Advanced Edition)**
**Môn học:** Lập trình Fullstack Development
**Sinh viên:** [HỌ VÀ TÊN CỦA BẠN] - [MSSV CỦA BẠN]

---

## 📖 Tổng Quan Dự Án

Hệ thống PCM Pro là giải pháp quản lý toàn diện cho CLB Pickleball, được xây dựng dựa trên kiến trúc **Clean Architecture** hiện đại. Hệ thống giải quyết các bài toán nghiệp vụ phức tạp như quản lý ví điện tử, đặt sân thời gian thực, tổ chức giải đấu chuyên nghiệp (Bracket) và tính điểm xếp hạng ELO tự động.

### 🌟 Tính Năng Nổi Bật

*   **Quản lý Hội viên & Ví điện tử (Fintech):** Nạp tiền, thanh toán tự động, lịch sử giao dịch minh bạch.
*   **Đặt sân thông minh (Smart Booking):** Lịch trực quan, kiểm tra trùng lịch, đặt định kỳ, tự động hủy nếu không thanh toán (Hangfire).
*   **Hệ thống Giải đấu (Tournament):** Hỗ trợ tạo giải đấu, chia bảng tự động, vẽ cây thi đấu (Bracket) trực quan.
*   **Xếp hạng ELO:** Tự động tính điểm trình độ dựa trên kết quả thi đấu thực tế.
*   **Công nghệ Real-time:** Cập nhật trạng thái sân và tỉ số trận đấu tức thì (SignalR).
*   **Hiệu năng cao:** Sử dụng Redis để cache dữ liệu và Hangfire cho các tác vụ nền.

---

## 🛠️ Công Nghệ Sử Dụng

### Backend (.NET 8)
*   **Framework:** ASP.NET Core Web API
*   **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
*   **Database:** SQL Server (Entity Framework Core Code First)
*   **Authentication:** JWT (JSON Web Token) & Identity
*   **Background Jobs:** Hangfire (Quét booking treo, tính toán định kỳ)
*   **Caching:** Redis (Cache cấu hình, Leaderboard)
*   **Real-time:** SignalR
*   **Containerization:** Docker

### Frontend (Vue.js 3)
*   **Framework:** Vue 3 (Composition API) + Vite
*   **State Management:** Pinia
*   **UI Framework:** Tailwind CSS
*   **HTTP Client:** Axios
*   **Router:** Vue Router
*   **Containerization:** Docker (Nginx)

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án

Bạn có thể chạy dự án theo 2 cách: **Docker Compose (Khuyên dùng)** hoặc **Chạy thủ công**.

### Cách 1: Chạy bằng Docker Compose (Nhanh nhất)

Cách này sẽ tự động khởi tạo toàn bộ môi trường gồm SQL Server, Redis, Backend API và Frontend.

#### 1. Yêu cầu
*   Cài đặt **Docker Desktop** và đảm bảo nó đang chạy (biểu tượng cá voi đứng yên).

#### 2. Thực hiện
1.  Mở Terminal tại thư mục gốc của dự án (nơi chứa file `docker-compose.yml`).
2.  Chạy lệnh sau để build và khởi động các container:
    ```bash
    docker-compose up -d --build
    ```
3.  Chờ vài phút để quá trình build hoàn tất và các service khởi động.

#### 3. Truy cập
*   **Frontend (Web App):** http://localhost:5173
*   **Backend (Swagger UI):** http://localhost:5000/swagger
*   **Hangfire Dashboard:** http://localhost:5000/hangfire

#### 4. Lưu ý quan trọng khi chạy Docker
*   **Xung đột cổng:** Đảm bảo bạn đã tắt các service SQL Server (port 1433) hoặc Redis (port 6379) đang chạy trên máy thật trước khi chạy lệnh Docker để tránh lỗi `Bind for 0.0.0.0:xxxx failed: port is already allocated`.
*   **Dữ liệu:** Dữ liệu SQL Server được lưu trong volume docker `sqlserver_data`.

---

### Cách 2: Chạy Thủ Công (Dành cho Dev/Debug)

#### 1. Yêu Cầu Môi Trường
*   .NET 8 SDK
*   Node.js (v18+)
*   SQL Server (Local)
*   Redis (Local hoặc Docker: `docker run -d -p 6379:6379 redis`)

#### 2. Cài Đặt Backend

1.  Di chuyển vào thư mục Backend:
    ```bash
    cd PickleballClubManagement
    ```
2.  Cấu hình chuỗi kết nối trong `PCM.API/appsettings.Development.json` nếu cần.
3.  Khôi phục các gói thư viện và chạy:
    ```bash
    dotnet restore
    dotnet run --project PCM.API
    ```
    *   Server sẽ khởi chạy tại: `http://localhost:5000`

#### 3. Cài Đặt Frontend

1.  Mở terminal mới và di chuyển vào thư mục Frontend:
    ```bash
    cd PickleballClubManagement_Frontend
    ```
2.  Cài đặt thư viện và chạy:
    ```bash
    npm install
    npm run dev
    ```
    *   Truy cập ứng dụng tại: `http://localhost:5173`

---

## 🔐 Tài Khoản Demo (Seeding Data)

Hệ thống đã được nạp sẵn dữ liệu mẫu để kiểm thử các quyền hạn khác nhau:

| Quyền (Role) | Email | Mật khẩu | Chức năng chính |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@pcm.com` | `Admin@123` | Quản trị toàn bộ hệ thống, cấu hình sân, giải đấu. |
| **Thủ Quỹ** | `treasurer@pcm.com` | `Treasurer@123` | Duyệt yêu cầu nạp tiền, xem báo cáo tài chính. |
| **Trọng Tài** | `referee@pcm.com` | `Referee@123` | Quản lý trận đấu, cập nhật tỉ số, kết thúc trận. |
| **Hội Viên** | `member1@pcm.com` | `Member@123` | Đặt sân, nạp tiền ví, xem lịch sử, xem giải đấu. |

---

## 📂 Cấu Trúc Source Code

```
PickleballClubManagement/          # Backend Solution
├── PCM.Domain/                    # Entities, Enums, Interfaces
├── PCM.Application/               # DTOs, Services Interfaces, Mappings
├── PCM.Infrastructure/            # DbContext, Repositories, Services Implementation
└── PCM.API/                       # Controllers, Program.cs, Middleware

PickleballClubManagement_Frontend/ # Frontend Vue.js
├── src/
│   ├── api/                       # Axios config
│   ├── components/                # Reusable components (Layout, etc.)
│   ├── stores/                    # Pinia State Management
│   ├── views/                     # Page Components (Login, Dashboard, Booking...)
│   └── router/                    # Vue Router config
```

---

## ✅ Tiêu Chí Tự Đánh Giá

*   [x] **Kiến trúc:** Tuân thủ Clean Architecture, tách biệt rõ ràng các tầng.
*   [x] **Nghiệp vụ:** Hoàn thiện luồng Ví điện tử (Nạp -> Duyệt -> Thanh toán), Đặt sân (Check trùng, trừ tiền), Giải đấu (Bracket tự động).
*   [x] **Công nghệ:** Tích hợp thành công Redis (Cache), Hangfire (Job ngầm), SignalR (Real-time notification).
*   [x] **UI/UX:** Giao diện Tailwind CSS hiện đại, responsive, thân thiện người dùng.

---

**© 2024 PCM Project. All rights reserved.**