# DANH SÁCH TÍNH NĂNG CÒN THIẾU SO VỚI ĐỀ BÀI

## Mức độ ưu tiên: CAO (Critical)

### 1. Wallet Approval UI cho Treasurer
**Vị trí:** `TransactionManagement.vue`
**Yêu cầu:** 
- Hiển thị danh sách pending deposits trong TransactionManagement
- Button "Duyệt" để Treasurer approve
- Đã có backend `/api/wallet/pending` và `/api/wallet/approve/{id}`

### 2. Available Slots Checker
**Vị trí:** `BookingCalendar.vue`
**Yêu cầu:**
- API endpoint: `GET /api/bookings/available-slots?courtId={id}&date={date}`
- Hiển thị các slot available trước khi đặt
- Prevent overlapping bookings

### 3. Countdown Timer cho Pending Bookings
**Vị trí:** `MyBookings.vue` hoặc Dashboard
**Yêu cầu:**
- Hiển thị countdown 15 phút cho bookings status = PendingPayment
- Cảnh báo trước khi hết hạn
- Auto refresh khi hết thời gian

### 4. Live Scoreboard Page
**Vị trí:** Tạo mới `LiveScoreboard.vue`
**Yêu cầu:**
- Trang public cho khán giả xem
- Real-time updates qua SignalR
- Hiển thị all ongoing matches

## Mức độ ưu tiên: TRUNG BÌNH (Important)

### 5. Export Transactions (Excel/PDF)
**Vị trí:** `TransactionManagement.vue`
**Yêu cầu:**
- Button Export
- API: `GET /api/transactions/export?format=excel|pdf`
- Download file

### 6. Activity Logs Viewer
**Vị trí:** Tạo mới `ActivityLogs.vue` (Admin only)
**Yêu cầu:**
- Hiển thị tất cả actions: Login, CRUD operations
- Filter by user, date, entity type
- Pagination

### 7. Match Scores Detail (Sets)
**Vị trí:** `MatchList.vue`
**Yêu cầu:**
- Track từng set: Set 1: 11-9, Set 2: 11-7
- Store in `MatchScores` table
- Display trong match detail modal

### 8. Team Battle Scoreboard
**Vị trí:** `TournamentBracket.vue` hoặc tách riêng
**Yêu cầu:**
- Hiển thị CurrentScore_TeamA vs CurrentScore_TeamB
- Progress bar đến TargetWins
- Real-time updates

### 9. Round Robin Standings
**Vị trí:** Trong Tournament detail
**Yêu cầu:**
- Bảng xếp hạng theo điểm cá nhân
- Win/Loss/Points columns
- Auto sort

### 10. Check-in UI
**Vị trí:** `TournamentDetail.vue`
**Yêu cầu:**
- Button "Check-in" cho participants
- Countdown before tournament starts
- Status: NotCheckedIn/CheckedIn/Eliminated

## Mức độ ưu tiên: THẤP (Nice to have)

### 11. FullCalendar Integration
**Thay thế:** BookingCalendar custom → FullCalendar library
**Lợi ích:** Better UX, drag-drop events

### 12. Bracket Editor
**Vị trí:** TournamentBracket.vue
**Yêu cầu:**
- Edit bracket manually
- Swap participants
- Re-generate bracket

### 13. Age Validation
**Vị trí:** MemberList.vue form
**Yêu cầu:**
- Calculate age from DateOfBirth
- Validate: >= 16 years old

### 14. Optimistic Locking UI
**Vị trí:** All edit forms
**Yêu cầu:**
- Catch 409 Conflict error from RowVersion
- Show modal: "Dữ liệu đã thay đổi, vui lòng tải lại"
- Reload fresh data

### 15. Wallet Balance Warnings
**Vị trí:** BookingCalendar, TournamentList
**Yêu cầu:**
- Check balance before booking/joining
- Show warning if insufficient
- Disable button + tooltip

### 16. Prize Distribution Confirm
**Vị trí:** TournamentList (Admin)
**Yêu cầu:**
- Button "Phân thưởng" khi tournament finished
- Auto create transactions for winners
- Confirmation modal

### 17. Redis Cache Indicator
**Vị trí:** AdminDashboard
**Yêu cầu:**
- Show cache hit/miss stats
- Button "Clear cache"
- Cache status for News, Courts, Rankings

### 18. Notification Center
**Vị trí:** Tạo NotificationCenter.vue
**Yêu cầu:**
- List all notifications
- Mark as read/unread
- Filter by type
- Pagination

## BACKEND CẦN BỔ SUNG

### API Endpoints thiếu:
- `GET /api/bookings/available-slots`
- `GET /api/transactions/export`
- `GET /api/activity-logs`
- `POST /api/tournaments/{id}/check-in`
- `POST /api/tournaments/{id}/distribute-prizes`
- `GET /api/matches/{id}/scores` (chi tiết sets)
- `POST /api/matches/{id}/scores` (thêm set score)

## KIẾN TRÚC CẦN REVIEW

### Redis Implementation:
- ✅ News caching
- ✅ Leaderboard (Sorted Sets)
- ❌ Courts caching
- ❌ Configurations caching

### Hangfire Jobs:
- ❌ Auto-cancel pending bookings (>15 mins)
- ❌ Daily summary report
- ❌ Recalculate rankings

### SignalR Hubs:
- ✅ NotificationHub
- ✅ BookingHub
- ✅ MatchHub
- ❌ TournamentHub (for bracket updates)

## TESTING CẦN LÀM

- [ ] Race condition testing (2 users book same slot)
- [ ] Wallet transaction atomicity
- [ ] SignalR connection recovery
- [ ] Token refresh flow
- [ ] Optimistic locking scenarios
- [ ] Recurring booking conflicts
- [ ] Team auto-divide algorithm
- [ ] Bracket generation edge cases

## TÀI LIỆU CẦN CẬP NHẬT

- [ ] API Documentation (Swagger)
- [ ] User Guide
- [ ] Deployment Guide (Docker)
- [ ] Database Schema Diagram
- [ ] Architecture Diagram
