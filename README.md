# HỆ THỐNG QUẢN LÝ CLB PICKLEBALL "VỢT THỦ PHỐ NÚI" (PCM) - PRO EDITION

**Bài Kiểm Tra 02 - Phiên bản Nâng cao (Advanced Edition)**  
**Môn học:** Lập trình Fullstack Development  
**Sinh viên:** Lê Tuấn Dũng - 1771020189

---

## 📖 Tổng Quan Dự Án

Hệ thống PCM Pro là giải pháp quản lý toàn diện cho CLB Pickleball, được xây dựng dựa trên kiến trúc **Clean Architecture** hiện đại với kiến trúc microservices. Hệ thống giải quyết các bài toán nghiệp vụ phức tạp như quản lý ví điện tử, đặt sân thời gian thực, tổ chức giải đấu chuyên nghiệp (Bracket), tính điểm xếp hạng ELO tự động và quản lý tài chính CLB.

### 🌟 Tính Năng Nổi Bật

#### 🏃 Quản lý Hội viên & Xác thực
*   **Đăng ký/Đăng nhập:** JWT Authentication với Identity Framework
*   **Phân quyền:** 4 roles (Admin, Treasurer, Referee, Member) với middleware authorization
*   **Quản lý hồ sơ:** Avatar, thông tin cá nhân, lịch sử thi đấu, ELO ranking
*   **Thống kê cá nhân:** Win rate, total matches, performance chart

#### 💰 Hệ thống Ví Điện tử (E-Wallet)
*   **Nạp tiền:** Member tạo yêu cầu → Treasurer duyệt → Auto cập nhật số dư
*   **Thanh toán tự động:** Đặt sân tự động trừ tiền ví, rollback nếu thất bại
*   **Lịch sử giao dịch:** Transaction history với filter theo loại/thời gian
*   **Bảo mật:** Transaction locking, concurrency handling với RowVersion

#### 📅 Đặt sân thông minh (Smart Booking)
*   **Lịch tuần trực quan:** Calendar view 7 ngày × 17 giờ (6:00-22:00)
*   **Kiểm tra trùng lịch:** Real-time conflict detection
*   **Multi-court support:** Đặt đồng thời nhiều sân
*   **Đặt định kỳ:** Recurring booking theo ngày trong tuần
*   **Auto-cancel:** Hangfire job tự động hủy booking chưa thanh toán sau 15 phút
*   **Real-time update:** SignalR broadcast khi có booking mới

#### 🏆 Hệ thống Giải đấu (Tournament Management)
*   **Tạo giải đấu:** Single elimination, Round-robin, Singles/Doubles
*   **Tự động chia bảng:** Auto-generate bracket dựa trên số người đăng ký
*   **Cây thi đấu (Bracket):** Visual knockout bracket tree
*   **Live scoring:** Trọng tài cập nhật tỉ số real-time qua SignalR
*   **Auto ELO calculation:** Tự động tính điểm ELO sau mỗi trận

#### 📊 Quản lý Tài chính CLB (Treasury)
*   **Dashboard tài chính:** Tổng thu/chi, biểu đồ dòng tiền
*   **Quản lý giao dịch:** CRUD transactions với categories
*   **Duyệt yêu cầu nạp tiền:** Approval workflow
*   **Báo cáo:** Export excel, PDF theo tháng/quý/năm

#### 🎯 Hệ thống Xếp hạng ELO
*   **Auto calculation:** Cập nhật ELO sau mỗi trận đấu
*   **Leaderboard:** Real-time ranking với Redis cache
*   **History tracking:** Lịch sử thay đổi điểm ELO theo thời gian

#### 📰 Quản lý Tin tức & Thông báo
*   **News management:** CRUD tin tức với pinned post, summary field
*   **Real-time notifications:** SignalR push notifications
*   **Notification center:** Mark as read, filter unread

---

## 🛠️ Công Nghệ & Thư Viện Sử Dụng

### Backend (.NET 8)
*   **Framework:** ASP.NET Core 8.0 Web API
*   **Architecture:** Clean Architecture (4 layers: Domain, Application, Infrastructure, API)
*   **Database:** 
    *   SQL Server 2022 (Entity Framework Core 8.0)
    *   Code First Migrations
    *   Repository Pattern & Unit of Work
*   **Authentication & Authorization:** 
    *   JWT (JSON Web Token)
    *   ASP.NET Core Identity
    *   Role-based & Policy-based authorization
*   **Background Jobs:** 
    *   Hangfire 1.8+ (SQL Server storage)
    *   Recurring jobs: Auto-cancel expired bookings, ELO recalculation
*   **Caching:** 
    *   Redis (StackExchange.Redis)
    *   Distributed cache cho leaderboard, news, tournament rankings
*   **Real-time Communication:** SignalR (WebSocket fallback)
*   **API Documentation:** Swagger/OpenAPI 3.0
*   **Logging:** Serilog + Application Insights
*   **Containerization:** Docker multi-stage build
*   **Packages:**
    *   AutoMapper 12.0 (DTO mapping)
    *   FluentValidation (Input validation)
    *   Newtonsoft.Json (JSON serialization)

### Frontend (Vue.js 3)
*   **Framework:** Vue 3.4+ (Composition API) + Vite 5.0
*   **State Management:** Pinia 2.1 (Store pattern)
*   **UI Framework & Components:**
    *   Tailwind CSS 3.4
    *   HeadlessUI
    *   Heroicons 2.0
*   **HTTP Client:** Axios 1.6 (with interceptors)
*   **Router:** Vue Router 4.2
*   **Form Handling:** VeeValidate + Yup
*   **Date/Time:** date-fns 3.0
*   **Notifications:** Vue Toastification
*   **Charts:** Chart.js 4.4 (for analytics)
*   **Containerization:** Docker (Nginx Alpine)
*   **Build Tools:**
    *   Vite (fast HMR)
    *   PostCSS (Tailwind processing)
    *   ESLint + Prettier

### DevOps & Infrastructure
*   **Container Orchestration:** Docker Compose
*   **Reverse Proxy:** Nginx
*   **Database Management:** SQL Server Management Studio (SSMS)
*   **API Testing:** Postman, Swagger UI
*   **Version Control:** Git

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án

Bạn có thể chạy dự án theo 2 cách: **Docker Compose (Khuyên dùng)** hoặc **Chạy thủ công**.

---

### ⭐ Cách 1: Chạy bằng Docker Compose (Recommended)

Cách này sẽ tự động khởi tạo toàn bộ môi trường gồm **SQL Server**, **Redis**, **Backend API**, **Frontend** và **Hangfire** trong các container riêng biệt.

#### 1. Yêu cầu
*   ✅ **Docker Desktop** 4.25+ (Windows/Mac) hoặc Docker Engine (Linux)
*   ✅ Đảm bảo Docker đang chạy (biểu tượng cá voi đứng yên, không xoay)
*   ✅ Tối thiểu 4GB RAM available cho Docker

#### 2. Cấu trúc Docker Services

```yaml
services:
  sqlserver:      # SQL Server 2022 (Port 1433)
  redis:          # Redis 7 (Port 6379)
  backend:        # .NET 8 API (Port 5000, 5001)
  frontend:       # Vue.js + Nginx (Port 5173)
```

#### 3. Các bước thực hiện

**Bước 1:** Mở Terminal/PowerShell tại thư mục gốc dự án
```bash
cd D:\FullStack\Test2\PickleballClubManagement
---

### 🔧 Cách 2: Chạy Thủ Công (Development Mode)

Dành cho developer muốn debug chi tiết hoặc phát triển tính năng mới.

#### 1. Yêu Cầu Môi Trường

**Backend:**
*   ✅ .NET 8.0 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
*   ✅ SQL Server 2019+ (Express/Developer/LocalDB)
*   ✅ SQL Server Management Studio (SSMS) hoặc Azure Data Studio
*   ✅ Redis (khuyên dùng Docker):
    ```bash
    docker run -d --name redis -p 6379:6379 redis:latest
    ```

**Frontend:**
*   ✅ Node.js 18+ & npm ([Download](https://nodejs.org/))
*   ✅ Git (for clone repository)

#### 2. Setup Database

**Option A: SQL Server LocalDB (nhẹ nhất)**
```bash
# Cài đặt LocalDB với .NET SDK
# Connection string mẫu:
Server=(localdb)\\mssqllocaldb;Database=PCM_189;Trusted_Connection=True;
```

**Option B: SQL Server Express**
```bash
# Download SQL Server 2022 Express
# Connection string mẫu:
Server=localhost\\SQLEXPRESS;Database=PCM_189;Trusted_Connection=True;
```

#### 3. Cài Đặt & Chạy Backend

**Bước 1:** Di chuyển vào thư mục Backend
```bash
cd PickleballClubManagement
```

**Bước 2:** Cấu hình Connection String

Mở file `PCM.API/appsettings.Development.json` và chỉnh sửa:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PCM_189;Trusted_Connection=True;TrustServerCertificate=True;",
    "HangfireConnection": "Server=YOUR_SERVER;Database=PCM_189_Hangfire;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "PCM_"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-chars-long",
    "Issuer": "PCM.API",
    "Audience": "PCM.Frontend",
    "ExpirationInMinutes": 60
  }
}
```

**Bước 3:** Restore packages & Run migrations
```bash
# Restore NuGet packages
dotnet restore

# Apply database migrations (tạo database + tables)
cd PCM.API
dotnet ef database update --project ../PCM.Infrastructure

# Chạy application (sẽ tự động seed data)
dotnet run
```

Backend sẽ khởi động tại:
*   HTTP: http://localhost:5000
*   HTTPS: https://localhost:5001
*   Swagger: http://localhost:5000/swagger

**Bước 4:** Verify backend
```bash
# Test API bằng curl
curl http://localhost:5000/api/courts

# Hoặc truy cập Swagger UI và test thử API
```

#### 4. Cài Đặt & Chạy Frontend

**Bước 1:** Mở terminal mới, di chuyển vào thư mục Frontend
```bash
cd PickleballClubManagement_Frontend
```

**Bước 2:** Cấu hình API Endpoint

Mở file `src/api/axiosClient.js` và đảm bảo:
```javascript
const axiosClient = axios.create({
  baseURL: 'http://localhost:5000/api',  // Backend API URL
  headers: {
    'Content-Type': 'application/json',
  },
});
```

**Bước 3:** Install dependencies & Run dev server
```bash
# Cài đặt node_modules (lần đầu hoặc khi có package mới)
npm install

# Chạy development server với HMR
npm run dev
```

Frontend sẽ khởi động tại: http://localhost:5173

**Bước 4:** Build for production (optional)
```bash
# Build static files
npm run build

# Preview production build
npm run preview
```

#### 5. Database Migrations (Khi có thay đổi schema)

```bash
cd PCM.API

# Tạo migration mới
dotnet ef migrations add MigrationName --project ../PCM.Infrastructure

# Apply migration
dotnet ef database update --project ../PCM.Infrastructure

# Rollback migration (nếu cần)
dotnet ef database update PreviousMigrationName --project ../PCM.Infrastructure

# Remove last migration (chưa apply)
dotnet ef migrations remove --project ../PCM.Infrastructure
```
 Chi Tiết

### Backend (.NET Clean Architecture)

```
PickleballClubManagement/
│
├── PCM.Domain/                           # 🔵 Domain Layer (Core Business Logic)
│   ├── Entities/                         # Entity models (POCO classes)
│   │   ├── Member.cs                     # Hội viên (UserId, FullName, ELO, Wallet)
│   │   ├── Booking.cs                    # Đặt sân (Court, Time, Status, Price)
│   │   ├── Court.cs                      # Sân (Name, PricePerHour, IsActive)
│   │   ├── Tournament.cs                 # Giải đấu (Type, Status, Prize)
│   │   ├── Match.cs                      # Trận đấu (Teams, Scores, WinningSide)
│   │   ├── Transaction.cs                # Giao dịch CLB
│   │   ├── WalletTransaction.cs          # Giao dịch ví member
│   │   ├── News.cs                       # Tin tức (Title, Summary, IsPinned)
│   │   ├── Notification.cs               # Thông báo real-time
│   │   ├── RefreshToken.cs               # JWT refresh tokens
│   │   └── ActivityLog.cs                # Audit log
│  📊 Database Schema Overview

### Core Tables

| Table | Mô tả | Key Fields |
|-------|-------|------------|
| **189_Members** | Hội viên CLB | UserId (FK), FullName, Email, RankELO, WalletBalance |
| **189_Courts** | Sân thi đấu | Name, Description, PricePerHour, IsActive |
| **189_Bookings** | Đặt sân | CourtId (FK), MemberId (FK), StartTime, EndTime, Status |
| **189_Tournaments** | Giải đấu | Title, Type, GameMode, Status, MaxParticipants, PrizePool |
| **189_Matches** | Trận đấu | TournamentId (FK), Team1/Team2 Scores, WinningSide, Date |
| **189_Participants** | Người tham gia giải | TournamentId (FK), MemberId (FK), Status, Seed |
| **189_Transactions** | Giao dịch CLB | CategoryId (FK), Amount, Description, CreatedBy |
| **189_WalletTransactions** | Giao dịch ví | MemberId (FK), Type, Amount, Status, ReferenceId |
| **189_News** | Tin tức | Title, Summary, Content, IsPinned, CreatedBy |
| **189_Notifications** | Thông báo | MemberId (FK), Title, Type, IsRead |
| **189_ActivityLogs** | Audit log | UserId, Action, Details, CreatedDate |
| **AspNetUsers** | Identity users | Email, PasswordHash, SecurityStamp |
| **AspNetRoles** | Roles | Admin, Treasurer, Referee, Member |

### Relationships
*   Member ↔ Booking (1:N)
*   Court ↔ Booking (1:N)
*   Member ↔ WalletTransaction (1:N)
*   Tournament ↔ Match (1:N)
*   Tournament ↔ Participant (M:N)
*   Member ↔ Notification (1:N)

---

## 🧪 Testing & Quality Assurance

### Test Accounts (Seeded Data)

Tất cả tài khoản đều có **ví điện tử** đã được nạp sẵn để test.

| Role | Email | Password | Wallet Balance | Test Cases |
|------|-------|----------|----------------|------------|
| **Admin** | admin@pcm.com | Admin@123 | ₫0 | Quản lý sân, tin tức, thành viên, giải đấu |
| **Treasurer** | treasurer@pcm.com | Treasurer@123 | ₫0 | Duyệt nạp tiền, xem báo cáo tài chính, quản lý giao dịch |
| **Referee** | referee@pcm.com | Referee@123 | ₫0 | Cập nhật tỉ số, quản lý trận đấu |
| **Member** | member1@pcm.com | Member@123 | ₫500,000 | Đặt sân, nạp tiền, tham gia giải, xem ELO |
| **Member** | nguyenvana@pcm.com | Member@123 | ₫500,000 | Testing user 2 |
| **Member** | tranthib@pcm.com | Member@123 | ₫500,000 | Testing user 3 |

### Test Scenarios

**✅ Authentication & Authorization**
- [x] Đăng ký thành viên mới → Auto role "Member" + ví ₫0
- [x] Đăng nhập với mỗi role → Kiểm tra menu hiển thị đúng
- [x] JWT token expiration → Auto redirect login
- [x] Unauthorized access → 403 Forbidden

**✅ Booking System**
- [x] Đặt sân trống → Trừ tiền tự động → Status "Confirmed"
- [x] Đặt sân trùng lịch → 400 Bad Request
- [x] Đặt sân với ví không đủ tiền → 400 Error
- [x] Hangfire auto-cancel booking chưa thanh toán sau 15 phút

**✅ Wallet & Transactions**
- [x] Member nạp tiền → Status "Pending" → Treasurer duyệt → Cập nhật số dư
- [x] Thanh toán booking → Tạo WalletTransaction type "Payment"
- [x] Xem lịch sử giao dịch → Pagination + Filter

**✅ Tournament System**
- [x] Admin tạo giải → Single Elimination 16 người
- [x] Member đăng ký giải → Trừ phí tham gia
- [x] Tự động chia bảng → Generate Bracket
- [x] Referee cập nhật tỉ số → SignalR push real-time
- [x] Trận kết thúc → Auto tính ELO

**✅ Real-time Features**
- [x] Đặt sân → SignalR broadcast → Cập nhật calendar
- [x] Cập nhật tỉ số → SignalR → Update scoreboard
- [x] Notification push → Bell icon bật đỏ

---

## 🚨 Known Issues & Limitations

### Hiện tại
*   ❌ Chưa có email service (SendGrid/SMTP) để gửi mail xác nhận
*   ❌ Chưa có payment gateway (VNPay/Momo) cho thanh toán online
*   ❌ Redis cache chưa có TTL config chi tiết
*   ❌ Chưa có unit tests & integration tests

### Future Improvements
*   [ ] Implement Google/Facebook OAuth login
*   [ ] Add export PDF reports (tournaments, transactions)
*   [ ] Mobile app (React Native/Flutter)
*   [ ] AI-powered bracket seeding dựa trên ELO
*   [ ] Multi-language support (i18n)
*   [ ] Advanced analytics dashboard với Chart.js
*   [ ] Push notifications (Firebase Cloud Messaging)

---

## 📈 Performance & Scalability

### Current Setup
*   **Database:** Indexed primary keys, foreign keys
*   **Caching:** Redis cho leaderboard, news, tournament rankings
*   **Background Jobs:** Hangfire xử lý tasks nặng (ELO calculation, booking cleanup)
*   **Real-time:** SignalR với WebSocket, fallback to Long Polling

### Load Test Results (Simulated)
*   Concurrent Users: 100
*   Avg Response Time: < 200ms (cached endpoints)
*   Booking Conflict Detection: < 50ms (SQL indexed query)

---

## 🛡️ Security Features

*   ✅ **Authentication:** JWT với refresh tokens, secure httpOnly cookies
*   ✅ **Authorization:** Role-based + Policy-based với [Authorize] attribute
*   ✅ **Password:** Hashed với Identity default (PBKDF2 + salt)
*   ✅ **SQL Injection:** Protected by EF Core parameterized queries
*   ✅ **XSS:** Sanitized inputs, CSP headers
*   ✅ **CORS:** Configured for specific origins only
*   ✅ **HTTPS:** Enforced in production (Nginx SSL termination)
*   ✅ **Concurrency:** Optimistic locking với RowVersion (Booking, WalletTransaction)

---

## 📝 API Documentation (Swagger)

Truy cập **http://localhost:5000/swagger** để xem đầy đủ API documentation.

### Key Endpoints

**Authentication**
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
GET  /api/auth/me
```

**Bookings**
```
GET    /api/bookings?pageNumber=1&pageSize=10
GET    /api/bookings/{id}
POST   /api/bookings
POST   /api/bookings/recurring
PUT    /api/bookings/{id}
DELETE /api/bookings/{id}
```

**Tournaments**
```
GET  /api/tournaments
GET  /api/tournaments/{id}
POST /api/tournaments
GET  /api/tournaments/{id}/bracket
POST /api/tournaments/{id}/start
```

**Wallet**
```
GET  /api/wallet/balance
GET  /api/wallet/transactions
POST /api/wallet/deposit
POST /api/wallet/withdraw
```

---

## 🤝 Contributing Guidelines

1. Fork repository
2. Tạo branch mới: `git checkout -b feature/AmazingFeature`
3. Commit changes: `git commit -m 'Add AmazingFeature'`
4. Push to branch: `git push origin feature/AmazingFeature`
5. Open Pull Request

---

## ✅ Tiêu Chí Tự Đánh Giá

### Backend
*   [x] **Kiến trúc:** Clean Architecture 4 layers, SOLID principles
*   [x] **Entity Framework:** Code First, Migrations, Repository Pattern
*   [x] **Authentication:** JWT + Identity + Role-based Authorization
*   [x] **Business Logic:** 
    *   [x] Wallet: Deposit workflow, auto payment, transaction locking
    *   [x] Booking: Conflict check, auto cancel expired bookings
    *   [x] Tournament: Bracket generation, ELO calculation
*   [x] **Background Jobs:** Hangfire recurring tasks
*   [x] **Caching:** Redis distributed cache với fallback
*   [x] **Real-time:** SignalR hub cho notifications & scoreboard
*   [x] **API:** RESTful design, Swagger documentation

### Frontend
*   [x] **Framework:** Vue 3 Composition API, Pinia state management
*   [x] **UI/UX:** Tailwind CSS responsive design
*   [x] **Authentication:** JWT interceptor, auto logout on 401
*   [x] **Features:**
    *   [x] Calendar booking với conflict detection UI
    *   [x] Tournament bracket tree visualization
    *   [x] Real-time notifications bell
    *   [x] Wallet history với pagination
*   [x] **Router Guards:** Role-based route protection

### DevOps
*   [x] **Containerization:** Docker multi-stage build
*   [x] **Orchestration:** Docker Compose với 4 services
*   [x] **Database:** Persistent volumes cho SQL Server
*   [x] **Configuration:** Environment-based settings

---

## 📞 Contact & Support

*   **Developer:** Lê Tuấn Dũng - 1771020189
*   **Project Repository:** [GitHub Link]
*   **Demo Video:** [YouTube Link]

---

**© 2026 PCM Project - Pickleball Club Management Systemory.cs                # Generic repository interface
│       └── IUnitOfWork.cs                # Unit of Work pattern
│
├── PCM.Application/                      # 🟢 Application Layer (Use Cases)
│   ├── DTOs/                             # Data Transfer Objects
│   │   ├── Auth/
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── RegisterRequestDto.cs
│   │   │   └── AuthResponseDto.cs
│   │   ├── Bookings/
│   │   │   ├── BookingDto.cs
│   │   │   ├── BookingCreateDto.cs       # CourtId, StartTime, EndTime
│   │   │   └── RecurringBookingDto.cs
│   │   ├── Courts/
│   │   │   └── CourtDto.cs
│   │   ├── Members/
│   │   │   ├── MemberDto.cs
│   │   │   └── MemberUpdateDto.cs
│   │   ├── Tournaments/
│   │   │   ├── TournamentDto.cs
│   │   │   ├── TournamentCreateDto.cs
│   │   │   └── BracketDto.cs             # Cây thi đấu
│   │   ├── Transactions/
│   │   │   └── TransactionDto.cs
│   │   ├── Wallet/
│   │   │   ├── WalletDepositRequestDto.cs
│   │   │   └── WalletTransactionDto.cs
│   │   └── Common/
│   │       ├── ApiResponse.cs            # Standardized response
│   │       └── PagedResult.cs            # Pagination wrapper
│   ├── Interfaces/                       # Service contracts
│   │   ├── IAuthService.cs
│   │   ├── IBookingService.cs
│   │   ├── ICourtService.cs
│   │   ├── IMemberService.cs
│   │   ├── INewsService.cs
│   │   ├── ITournamentService.cs
│   │   ├── ITransactionService.cs
│   │   ├── IWalletService.cs
│   │   ├── INotificationService.cs
│   │   └── IActivityLogService.cs
│   └── Mappings/
│       └── MappingProfile.cs             # AutoMapper configuration
│
├── PCM.Infrastructure/                   # 🟡 Infrastructure Layer (External Concerns)
│   ├── Data/
│   │   ├── ApplicationDbContext.cs       # EF Core DbContext
│   │   ├── DbInitializer.cs              # Seed initial data
│   │   └── Migrations/                   # EF Core migrations
│   ├── Repositories/
│   │   ├── Repository.cs                 # Generic repository implementation
│   │   └── UnitOfWork.cs                 # Unit of Work implementation
│   └── Services/                         # Business logic implementation
│       ├── AuthService.cs                # JWT, Identity, Login/Register
│       ├── BookingService.cs             # Conflict check, auto payment
│       ├── CourtService.cs
│       ├── MemberService.cs              # ELO calculation, leaderboard
│       ├── NewsService.cs                # Redis cache pinned news
│       ├── TournamentService.cs          # Bracket generation, SignalR
│       ├── TransactionService.cs
│       ├── WalletService.cs              # Deposit, withdrawal, balance check
│       ├── NotificationService.cs        # SignalR broadcast
│       └── ActivityLogService.cs
│
└── PCM.API/                              # 🔴 Presentation Layer (API Endpoints)
    ├── Controllers/                      # API Controllers
    │   ├── AuthController.cs             # POST /login, /register, /refresh-token
    │   ├── BookingsController.cs         # CRUD bookings, recurring booking
    │   ├── CourtsController.cs           # CRUD courts
    │   ├── MatchesController.cs          # GET matches, update scores
    │   ├── MembersController.cs          # CRUD members, GET /count
    │   ├── NewsController.cs             # CRUD news, GET pinned
    │   ├── NotificationsController.cs    # GET notifications, mark read
    │   ├── TournamentsController.cs      # CRUD tournaments, GET bracket
    │   ├── TransactionsController.cs     # CRUD transactions, reports
    │   ├── TransactionCategoriesController.cs
    │   └── WalletController.cs           # Deposit, withdraw, history
    ├── Hubs/
    │   └── ScoreboardHub.cs              # SignalR hub for real-time updates
    ├── Middleware/
    │   └── ExceptionMiddleware.cs        # Global error handling
    ├── Program.cs                        # Application entry point, DI setup
    ├── appsettings.json                  # Configuration (ConnectionStrings, JWT)
    ├── Dockerfile                        # Docker build instructions
    └── PCM.API.csproj                    # Project file
```

### Frontend (Vue.js 3)

```
PickleballClubManagement_Frontend/
│
├── public/                               # Static assets
│   └── favicon.ico
│
├── src/
│   ├── api/
│   │   └── axiosClient.js                # Axios instance với interceptors
│   │
│   ├── assets/                           # Images, fonts, global CSS
│   │   └── main.css                      # Tailwind imports
│   │
│   ├── components/                       # Reusable components
│   │   ├── layout/
│   │   │   ├── MainLayout.vue            # Sidebar + Header layout
│   │   │   └── NotificationBell.vue      # Real-time notification icon
│   │   └── ui/                           # UI components (if any)
│   │
│   ├── router/
│   │   └── index.js                      # Vue Router config với route guards
│   │
│   ├── stores/                           # Pinia stores
│   │   ├── auth.js                       # Authentication state (user, token)
│   │   ├── booking.js                    # Booking CRUD, courts list
│   │   ├── tournament.js                 # Tournament CRUD, bracket
│   │   ├── notification.js               # Real-time notifications
│   │   └── ...
│   │
│   ├── views/                            # Page components
│   │   ├── auth/
│   │   │   └── Login.vue                 # Login form
│   │   ├── bookings/
│   │   │   └── BookingCalendar.vue       # Weekly calendar view
│   │   ├── courts/
│   │   │   └── CourtList.vue             # Court management (Admin)
│   │   ├── members/
│   │   │   └── MemberList.vue            # Member list với pagination
│   │   ├── news/
│   │   │   └── NewsList.vue              # News CRUD (Admin)
│   │   ├── referee/
│   │   │   └── MatchList.vue             # Match scoring (Referee)
│   │   ├── tournaments/
│   │   │   ├── TournamentList.vue        # Tournament list + create modal
│   │   │   └── TournamentBracket.vue     # Knockout bracket tree
│   │   ├── treasury/
│   │   │   └── TransactionManagement.vue # Finance dashboard (Treasurer)
│   │   ├── wallet/
│   │   │   └── MyWallet.vue              # Wallet balance, deposit, history
│   │   └── Dashboard.vue                 # Home dashboard
│   │
│   ├── App.vue                           # Root component
│   └── main.js                           # Vue app initialization
│
├── .env                                  # Environment variables
├── vite.config.js                        # Vite configuration
├── tailwind.config.js                    # Tailwind CSS config
├── package.json                          # NPM dependencies
├── Dockerfile                            # Docker build for production
└── nginx.conf                            # Nginx config for Docker
# Hoặc test Redis
redis-cli ping  # Response: PONG
```

**Lỗi: "Port 5000 already in use"**
```bash
# Windows: Tìm process đang dùng port
netstat -ano | findstr :5000

# Kill process
taskkill /PID <PID> /F

# Hoặc thay đổi port trong launchSettings.json
```

**Lỗi: "npm install fails"**
```bash
# Clear cache và reinstall
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
``
#### 5. Lưu ý quan trọng

**🔴 Xung đột cổng:**
Nếu gặp lỗi `port is already allocated`, hãy tắt các service sau trên máy local:
```bash
# Windows: Kiểm tra process đang chiếm port
netstat -ano | findstr ":1433"
netstat -ano | findstr ":6379"
netstat -ano | findstr ":5000"

# Kill process (thay <PID> bằng số PID thực tế)
taskkill /PID <PID> /F
```

**💾 Dữ liệu persistent:**
*   SQL Server: Volume `sqlserver_data` (dữ liệu không mất khi restart)
*   Redis: In-memory (mất dữ liệu khi restart - chỉ dùng cache)

**🔄 Commands hữu ích:**
```bash
# Dừng tất cả containers (không xóa data)
docker-compose stop

# Khởi động lại
docker-compose start

# Dừng và XÓA containers + networks (giữ lại volumes)
docker-compose down

# Xóa hoàn toàn (bao gồm volumes)
docker-compose down -v

# Rebuild một service cụ thể
docker-compose up -d --build backend

# Xem resource usage
docker stats
```

#### 6. Troubleshooting Docker

**Lỗi: "SQL Server container khởi động chậm"**
```bash
# Chờ SQL Server ready (có thể mất 1-2 phút lần đầu)
docker-compose logs -f sqlserver

# Khi thấy "SQL Server is now ready for client connections" là OK
```

**Lỗi: "Backend không kết nối được SQL Server"**
```bash
# Kiểm tra connection string trong docker-compose.yml
# Đảm bảo backend depends_on: sqlserver và có health check
```

**Lỗi: "Redis connection timeout"**
```bash
# Backend sẽ tự động fallback khi Redis chưa ready
# Kiểm tra Redis logs:
docker-compose logs redis
```

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