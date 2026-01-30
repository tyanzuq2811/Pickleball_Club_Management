# Email Service Setup Guide

## Cấu hình Email SMTP với Gmail

### Bước 1: Tạo Google App Password

1. Truy cập [Google Account Security](https://myaccount.google.com/security)
2. Bật **2-Step Verification** (nếu chưa bật)
3. Tìm mục **App passwords** trong Security settings
4. Chọn **Select app** → **Mail**
5. Chọn **Select device** → **Other (Custom name)**
6. Nhập tên: `Pickleball Club Management`
7. Click **Generate**
8. Copy 16-ký tự app password (dạng: `xxxx xxxx xxxx xxxx`)

### Bước 2: Cấu hình appsettings.json

Mở file `PCM.API/appsettings.json` và cập nhật:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromPassword": "xxxx xxxx xxxx xxxx",
    "EnableSsl": true
  }
}
```

**Lưu ý:**
- `FromEmail`: Địa chỉ Gmail của bạn
- `FromPassword`: App password vừa tạo (16 ký tự, bao gồm dấu cách)
- **KHÔNG** dùng mật khẩu Gmail thông thường

### Bước 3: Bảo mật Secrets

Với production, sử dụng **User Secrets** hoặc **Environment Variables**:

#### Option 1: User Secrets (Development)

```bash
cd PCM.API
dotnet user-secrets init
dotnet user-secrets set "Email:FromEmail" "your-email@gmail.com"
dotnet user-secrets set "Email:FromPassword" "your-app-password"
```

#### Option 2: Environment Variables (Production)

```bash
export Email__FromEmail="your-email@gmail.com"
export Email__FromPassword="your-app-password"
```

Hoặc trong `launchSettings.json`:

```json
{
  "environmentVariables": {
    "Email__FromEmail": "your-email@gmail.com",
    "Email__FromPassword": "your-app-password"
  }
}
```

## Các loại Email được hỗ trợ

### 1. Booking Confirmation
**Trigger:** Sau khi thanh toán booking thành công  
**Template:** HTML với thông tin sân, giờ chơi, tổng tiền  
**Method:** `SendBookingConfirmationAsync()`

### 2. Payment Success
**Trigger:** Thanh toán transaction thành công  
**Template:** Xác nhận giao dịch, số tiền, category  
**Method:** `SendPaymentSuccessAsync()`

### 3. Wallet Deposit Notification
**Trigger:** Admin approve/reject nạp tiền vào ví  
**Template:** Trạng thái nạp tiền (Approved/Rejected)  
**Method:** `SendWalletDepositNotificationAsync()`

### 4. Booking Reminder
**Trigger:** Hangfire job chạy mỗi giờ, gửi trước 1 giờ  
**Template:** Nhắc nhở booking sắp diễn ra  
**Method:** `SendBookingReminderAsync()`

## Testing Email Service

### Test với Development Console

Thêm vào `Program.cs` (temporary):

```csharp
app.MapGet("/test-email", async (IEmailService emailService) =>
{
    await emailService.SendEmailAsync(new EmailDto
    {
        To = "test@example.com",
        Subject = "Test Email",
        Body = "<h1>Hello from Pickleball Club!</h1>"
    });
    return Results.Ok("Email sent!");
});
```

Truy cập: `https://localhost:7xxx/test-email`

### Test với Hangfire Dashboard

1. Truy cập: `https://localhost:7xxx/hangfire`
2. Tìm job **send-booking-reminders**
3. Click **Trigger now** để chạy thủ công
4. Kiểm tra logs và email inbox

## Troubleshooting

### Lỗi: "Invalid credentials"
- Kiểm tra lại App Password đã copy đúng chưa
- Xác nhận 2-Step Verification đã bật
- Thử tạo lại App Password mới

### Lỗi: "Unable to read data from the transport connection"
- Kiểm tra firewall cho phép port 587
- Thử đổi sang port 465 với SSL/TLS

### Email không đến
- Kiểm tra Spam folder
- Xác nhận Gmail account không bị locked
- Kiểm tra logs trong BackgroundJobService

### Lỗi: "SMTP server requires secure connection"
- Xác nhận `EnableSsl: true` trong config
- Kiểm tra port 587 (TLS) hoặc 465 (SSL)

## Alternative SMTP Providers

### SendGrid
```json
{
  "Email": {
    "SmtpServer": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "FromEmail": "noreply@yourdomain.com",
    "FromPassword": "your-sendgrid-api-key",
    "EnableSsl": true
  }
}
```

### Mailgun
```json
{
  "Email": {
    "SmtpServer": "smtp.mailgun.org",
    "SmtpPort": 587,
    "FromEmail": "noreply@yourdomain.com",
    "FromPassword": "your-mailgun-smtp-password",
    "EnableSsl": true
  }
}
```

### Office 365
```json
{
  "Email": {
    "SmtpServer": "smtp.office365.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@yourdomain.com",
    "FromPassword": "your-password",
    "EnableSsl": true
  }
}
```

## Production Best Practices

1. **KHÔNG commit credentials vào Git**
   - Thêm `appsettings.Production.json` vào `.gitignore`
   - Dùng Azure Key Vault, AWS Secrets Manager

2. **Rate Limiting**
   - Gmail: 500 emails/day (free)
   - SendGrid: 100 emails/day (free tier)
   - Mailgun: 5,000 emails/month (free)

3. **Email Queue**
   - Hangfire đã tích hợp queue tự động
   - Retry failed emails: `[AutomaticRetry(Attempts = 3)]`

4. **Monitoring**
   - Kiểm tra Hangfire Dashboard thường xuyên
   - Setup alerts cho failed jobs
   - Track email delivery rates

5. **Templates**
   - Customize HTML templates trong `EmailService.cs`
   - Sử dụng logo, branding của club
   - Test trên mobile và desktop

## Security Checklist

- [x] 2-Step Verification enabled
- [x] Using App Password (not account password)
- [x] Credentials in User Secrets / Environment Variables
- [x] appsettings.Production.json in .gitignore
- [x] SSL/TLS enabled for SMTP
- [x] Email service errors don't crash application
- [ ] Setup email logging/monitoring
- [ ] Configure SPF/DKIM records (for custom domain)

---

**Cần hỗ trợ?** Liên hệ team DevOps hoặc kiểm tra logs tại `/hangfire`
