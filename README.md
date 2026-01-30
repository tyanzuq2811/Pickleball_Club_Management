# HỆ THỐNG QUẢN LÝ CLB PICKLEBALL "VỢT THỦ PHỐ NÚI" (PCM)

**Môn học:** Lập trình Fullstack Development  
**Sinh viên:** Lê Tuấn Dũng - 1771020189

---

## 📖 Tổng Quan Dự Án

Hệ thống PCM là giải pháp quản lý toàn diện cho CLB Pickleball, được xây dựng trên kiến trúc **Clean Architecture** với công nghệ **.NET 8** và **Vue.js 3**.

### 🌟 Tính Năng Chính

- 🏃 **Quản lý Hội viên:** CRUD, phân quyền 4 roles (Admin, Treasurer, Referee, Member)
- 💰 **Ví điện tử:** Nạp tiền, thanh toán tự động, lịch sử giao dịch
- 📅 **Đặt sân:** Calendar view, kiểm tra trùng lịch real-time, auto-cancel
- 🏆 **Giải đấu:** Bracket tournament, ELO ranking, live scoring
- 📊 **Tài chính CLB:** Thu chi, báo cáo, dashboard theo role
- 📰 **Tin tức & Thông báo:** CRUD news, real-time notifications (SignalR)

---

## 🛠️ Công Nghệ Sử Dụng

| Layer | Công nghệ |
|-------|-----------|
| **Backend** | .NET 8, EF Core, SQL Server, Redis, Hangfire, SignalR |
| **Frontend** | Vue 3, Vite, Pinia, Tailwind CSS, Axios |
| **DevOps** | Docker, Docker Compose, Nginx |

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy

### ⭐ Cách 1: Docker Compose (Khuyên dùng)

#### Yêu cầu
- Docker Desktop 4.25+ (Windows/Mac) hoặc Docker Engine (Linux)
- Tối thiểu 4GB RAM cho Docker

#### Bước 1: Tạo file cấu hình `.env`

```bash
# Copy file mẫu
cp .env.example .env
```

Mở file `.env` và điền giá trị:

```env
# BẮT BUỘC
DB_PASSWORD=MyP@ssw0rd!                    # Password SQL Server (phải mạnh)
JWT_KEY=YourSecretKeyAtLeast32Characters   # Khóa bí mật JWT (≥32 ký tự)

# TÙY CHỌN (nếu muốn gửi email)
EMAIL_FROM=your-email@gmail.com
EMAIL_PASSWORD=your-gmail-app-password
```

> ⚠️ **Lưu ý:** DB_PASSWORD phải có chữ hoa + chữ thường + số + ký tự đặc biệt

#### Bước 2: Chạy Docker Compose

```bash
# Build và khởi động tất cả services
docker-compose up -d --build

# Xem logs (chờ SQL Server ready ~1-2 phút)
docker-compose logs -f

# Kiểm tra containers đang chạy
docker-compose ps
```

#### Bước 3: Truy cập ứng dụng

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| Backend API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Hangfire Dashboard | http://localhost:5000/hangfire |

#### Docker Commands hữu ích

```bash
# Dừng tất cả
docker-compose down

# Xóa hoàn toàn (bao gồm database)
docker-compose down -v

# Rebuild một service
docker-compose up -d --build backend

# Xem logs một service
docker-compose logs -f backend
```

---

### 🔧 Cách 2: Chạy Thủ Công (Development)

#### Yêu cầu
- .NET 8 SDK
- Node.js 18+
- SQL Server 2019+ (Express/Developer)
- Redis (tùy chọn): `docker run -d -p 6379:6379 redis`

#### Bước 1: Cấu hình Backend

**File:** `PickleballClubManagement/PCM.API/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PCM_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "HangfireConnection": "Server=YOUR_SERVER;Database=PCM_Hangfire;Trusted_Connection=True;TrustServerCertificate=True;",
    "RedisConnection": "localhost:6379"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyForJwtTokenGenerationMustBeLongEnough",
    "Issuer": "https://localhost:7000",
    "Audience": "https://localhost:7000",
    "ExpireHours": 24
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromPassword": "your-app-password",
    "EnableSsl": true
  }
}
```

> Thay `YOUR_SERVER` bằng tên SQL Server của bạn (ví dụ: `localhost\\SQLEXPRESS`)

#### Bước 2: Chạy Backend

```bash
cd PickleballClubManagement/PCM.API

# Restore packages
dotnet restore

# Chạy migrations (tạo database)
dotnet ef database update --project ../PCM.Infrastructure

# Chạy server
dotnet run
```

✅ Backend chạy tại: http://localhost:5000

#### Bước 3: Cấu hình Frontend

**File:** `PickleballClubManagement_Frontend/.env.local`

```env
VITE_API_URL=http://localhost:5000/api
```

#### Bước 4: Chạy Frontend

```bash
cd PickleballClubManagement_Frontend

# Cài đặt packages
npm install

# Chạy dev server
npm run dev
```

✅ Frontend chạy tại: http://localhost:5173

---

## 🔐 Tài Khoản Test

| Role | Họ tên | Email | Mật khẩu |
|------|--------|-------|----------|
| **Admin** | Trần Minh Quân | `admin@pcm.com` | `Admin@123` |
| **Treasurer** | Nguyễn Thị Hồng Nhung | `treasurer@pcm.com` | `Treasurer@123` |
| **Referee** | Phạm Văn Hùng | `referee@pcm.com` | `Referee@123` |
| **Member** | Lê Tuấn Dũng | `letuandung@pcm.com` | `Member@123` |

### Thêm một số Member khác

| Họ tên | Email | Mật khẩu | Ví |
|--------|-------|----------|-----|
| Nguyễn Hoàng Nam | nguyenhoangnam@pcm.com | Member@123 | 500,000đ |
| Trần Thị Thanh Hà | tranthithanhha@pcm.com | Member@123 | 500,000đ |
| Lê Minh Khôi | leminhkhoi@pcm.com | Member@123 | 500,000đ |
| Phạm Quốc Bảo | phamquocbao@pcm.com | Member@123 | 500,000đ |

---

## 🧪 Hướng Dẫn Test Theo Role

### 👑 Admin (`admin@pcm.com`)
1. Đăng nhập → Dashboard tổng quan hệ thống
2. **Quản lý thành viên:** Xem danh sách, tìm kiếm
3. **Quản lý sân:** Thêm/sửa/xóa sân
4. **Quản lý giải đấu:** Tạo giải, chia bảng, tạo bracket
5. **Quản lý tin tức:** CRUD tin tức, ghim tin
6. **Xem tài chính:** Dashboard tổng quan quỹ CLB

### 💰 Treasurer (`treasurer@pcm.com`)
1. Đăng nhập → Dashboard tài chính
2. **Quản lý thu chi:** Thêm giao dịch thu/chi
3. **Danh mục giao dịch:** CRUD categories
4. **Duyệt nạp tiền:** Xem yêu cầu nạp tiền từ Member → Duyệt/Từ chối
5. **Báo cáo:** Xem thống kê theo tháng/quý

### ⚖️ Referee (`referee@pcm.com`)
1. Đăng nhập → Dashboard trọng tài
2. **Lịch trận đấu:** Xem trận được phân công
3. **Cập nhật tỷ số:** Nhập điểm từng set → Kết thúc trận
4. **Live scoring:** Tỷ số cập nhật real-time qua SignalR

### 🎾 Member (`letuandung@pcm.com`)
1. Đăng nhập → Dashboard cá nhân (Ví, ELO, Win Rate)
2. **Nạp tiền ví:** Tạo yêu cầu → Chờ Treasurer duyệt
3. **Đặt sân:** Chọn ngày giờ → Thanh toán từ ví → Xác nhận
4. **Xem lịch đặt sân:** My Bookings
5. **Tham gia giải đấu:** Đăng ký → Đóng phí → Chờ bracket

---

## 📧 Cấu Hình Email (Gmail)

### Bước 1: Bật 2FA và tạo App Password

1. Vào [Google Account](https://myaccount.google.com/) → **Security**
2. Bật **2-Step Verification**
3. Vào **App passwords** → Tạo password mới
4. Chọn "Mail" + "Windows Computer" → **Generate**
5. Copy 16 ký tự password

### Bước 2: Cấu hình

**Local (appsettings.Development.json):**
```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "FromEmail": "your-email@gmail.com",
  "FromPassword": "xxxx xxxx xxxx xxxx",
  "EnableSsl": true
}
```

**Docker (.env):**
```env
EMAIL_FROM=your-email@gmail.com
EMAIL_PASSWORD=xxxx xxxx xxxx xxxx
```

### Khi nào email được gửi?
- ✉️ Đặt sân thành công → Email xác nhận
- ✉️ Nạp tiền được duyệt → Email thông báo
- ✉️ Nhắc nhở trước giờ đặt sân (Hangfire job)

---

## 📂 Cấu Trúc Project

```
Test2/
├── .env.example                    # Template biến môi trường Docker
├── .env                            # Biến môi trường thật (KHÔNG commit)
├── docker-compose.yml              # Docker orchestration
│
├── PickleballClubManagement/       # Backend .NET 8
│   ├── PCM.Domain/                 # Entities, Enums, Interfaces
│   ├── PCM.Application/            # DTOs, Service Interfaces
│   ├── PCM.Infrastructure/         # DbContext, Repositories, Services
│   └── PCM.API/                    # Controllers, Program.cs
│       ├── appsettings.json        # Config mặc định (commit)
│       ├── appsettings.Development.json  # Config dev (KHÔNG commit)
│       └── Dockerfile
│
└── PickleballClubManagement_Frontend/  # Frontend Vue 3
    ├── src/
    │   ├── api/axiosClient.js      # Axios config
    │   ├── stores/                 # Pinia stores
    │   ├── views/                  # Pages
    │   └── components/             # Reusable components
    ├── .env.local                  # API URL (KHÔNG commit)
    ├── nginx.conf                  # Nginx config cho Docker
    └── Dockerfile
```

---

## 🔒 Bảo Mật

### Files KHÔNG được commit lên Git:
- `.env` - Chứa DB password, JWT key
- `appsettings.Development.json` - Chứa connection strings thật
- `appsettings.Production.json` - Chứa config production
- `.env.local` - Chứa API URL

### Files được commit (an toàn):
- `.env.example` - Template hướng dẫn
- `appsettings.json` - Chứa placeholder values
- `appsettings.Development.Example.json` - Template hướng dẫn

---

## 🐛 Troubleshooting

### Lỗi "Port already in use"
```bash
# Windows - Tìm process
netstat -ano | findstr :5000

# Kill process
taskkill /PID <PID> /F
```

### Lỗi SQL Server connection (Docker)
```bash
# Chờ SQL Server khởi động (~1-2 phút lần đầu)
docker-compose logs -f sqlserver

# Khi thấy "SQL Server is now ready" là OK
```

### Lỗi "npm install fails"
```bash
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

### Reset Database
```bash
cd PickleballClubManagement/PCM.API

# Xóa database
dotnet ef database drop --force --project ../PCM.Infrastructure

# Tạo lại
dotnet ef database update --project ../PCM.Infrastructure

# Chạy lại (sẽ tự seed data)
dotnet run
```

---

## 📊 API Endpoints Chính

### Authentication
```
POST /api/auth/login          # Đăng nhập
POST /api/auth/register       # Đăng ký
GET  /api/auth/me             # Thông tin user hiện tại
```

### Members
```
GET  /api/members             # Danh sách (Admin, Treasurer)
GET  /api/members/me          # Profile cá nhân
GET  /api/members/top-ranking # Leaderboard ELO
```

### Bookings
```
GET  /api/bookings/my-bookings  # Lịch đặt của tôi
POST /api/bookings              # Tạo booking mới
```

### Wallet
```
GET  /api/wallet/balance        # Số dư ví
POST /api/wallet/deposit        # Yêu cầu nạp tiền
POST /api/wallet/approve/{id}   # Duyệt nạp tiền (Treasurer)
```

### Tournaments
```
GET  /api/tournaments           # Danh sách giải
GET  /api/tournaments/{id}/bracket  # Cây thi đấu
POST /api/tournaments/{id}/register # Đăng ký tham gia
```

### Transactions (Treasurer only)
```
GET  /api/transactions          # Danh sách giao dịch
POST /api/transactions          # Tạo giao dịch
GET  /api/transactions/summary  # Thống kê (Admin, Treasurer)
```

📖 **Xem đầy đủ:** http://localhost:5000/swagger

---

## ✅ Checklist Test

- [ ] Đăng nhập với 4 roles khác nhau
- [ ] Admin: Tạo sân mới, tạo giải đấu, đăng tin tức
- [ ] Treasurer: Thêm giao dịch, duyệt nạp tiền
- [ ] Referee: Cập nhật tỷ số trận đấu
- [ ] Member: Nạp tiền, đặt sân, đăng ký giải đấu
- [ ] Kiểm tra SignalR: Cập nhật real-time khi có booking/score mới
- [ ] Kiểm tra Hangfire: Auto-cancel booking quá hạn

---

**© 2026 PCM Project - Pickleball Club Management System**
