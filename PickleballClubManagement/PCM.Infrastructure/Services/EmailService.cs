using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PCM.Application.DTOs.Email;
using PCM.Application.Interfaces;

namespace PCM.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _fromPassword;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@pickleballclub.com";
        _fromPassword = _configuration["Email:FromPassword"] ?? "";
        _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
    }

    public async Task SendEmailAsync(EmailDto emailDto)
    {
        try
        {
            using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_fromEmail, _fromPassword),
                EnableSsl = _enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, "Pickleball Club Management"),
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = emailDto.IsHtml
            };

            mailMessage.To.Add(emailDto.To);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {emailDto.To}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {emailDto.To}");
            // Don't throw - email failures shouldn't break the app
        }
    }

    public async Task SendBookingConfirmationAsync(string email, BookingConfirmationEmailDto dto)
    {
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #2563eb;'>🎾 Xác nhận đặt sân thành công!</h2>
                    <p>Xin chào <strong>{dto.MemberName}</strong>,</p>
                    <p>Booking của bạn đã được xác nhận thành công với thông tin sau:</p>
                    
                    <div style='background-color: #f3f4f6; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                        <p><strong>🏟️ Sân:</strong> {dto.CourtName}</p>
                        <p><strong>📅 Ngày giờ:</strong> {dto.StartTime:dd/MM/yyyy HH:mm} - {dto.EndTime:HH:mm}</p>
                        <p><strong>💰 Tổng tiền:</strong> {dto.TotalPrice:N0} VNĐ</p>
                        <p><strong>🔖 Mã booking:</strong> {dto.BookingId}</p>
                    </div>
                    
                    <p>Vui lòng đến sân trước giờ đặt <strong>10 phút</strong> để làm thủ tục check-in.</p>
                    
                    <p style='color: #059669; font-weight: bold;'>✓ Chúc bạn có trận đấu vui vẻ!</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #6b7280;'>
                        Email này được gửi tự động từ hệ thống Pickleball Club Management.<br>
                        Vui lòng không trả lời email này.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(new EmailDto
        {
            To = email,
            Subject = "Xác nhận đặt sân - Pickleball Club",
            Body = body,
            IsHtml = true
        });
    }

    public async Task SendPaymentSuccessAsync(string email, PaymentSuccessEmailDto dto)
    {
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #059669;'>✅ Thanh toán thành công!</h2>
                    <p>Xin chào <strong>{dto.MemberName}</strong>,</p>
                    <p>Thanh toán của bạn đã được xử lý thành công.</p>
                    
                    <div style='background-color: #f0fdf4; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                        <p><strong>💳 Phương thức:</strong> {dto.PaymentMethod}</p>
                        <p><strong>💰 Số tiền:</strong> <span style='color: #059669; font-size: 24px; font-weight: bold;'>{dto.Amount:N0} VNĐ</span></p>
                        <p><strong>🔖 Mã giao dịch:</strong> {dto.TransactionId}</p>
                        <p><strong>📅 Thời gian:</strong> {dto.PaymentDate:dd/MM/yyyy HH:mm:ss}</p>
                    </div>
                    
                    <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #6b7280;'>
                        Email này được gửi tự động từ hệ thống Pickleball Club Management.<br>
                        Vui lòng không trả lời email này.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(new EmailDto
        {
            To = email,
            Subject = "Thanh toán thành công - Pickleball Club",
            Body = body,
            IsHtml = true
        });
    }

    public async Task SendWalletDepositNotificationAsync(string email, WalletDepositEmailDto dto)
    {
        var statusColor = dto.Status == "Approved" ? "#059669" : "#dc2626";
        var statusText = dto.Status == "Approved" ? "✅ Đã duyệt" : "❌ Từ chối";

        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: {statusColor};'>Thông báo nạp tiền vào ví</h2>
                    <p>Xin chào <strong>{dto.MemberName}</strong>,</p>
                    <p>Yêu cầu nạp tiền của bạn đã được xử lý với trạng thái: <strong style='color: {statusColor};'>{statusText}</strong></p>
                    
                    <div style='background-color: #f3f4f6; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                        <p><strong>💰 Số tiền:</strong> {dto.Amount:N0} VNĐ</p>
                        <p><strong>📅 Ngày nạp:</strong> {dto.DepositDate:dd/MM/yyyy HH:mm}</p>
                        <p><strong>📊 Trạng thái:</strong> <span style='color: {statusColor};'>{statusText}</span></p>
                    </div>
                    
                    <p>{(dto.Status == "Approved" ? "Số dư ví của bạn đã được cập nhật." : "Vui lòng kiểm tra lại thông tin và thử lại.")}</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #6b7280;'>
                        Email này được gửi tự động từ hệ thống Pickleball Club Management.<br>
                        Vui lòng không trả lời email này.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(new EmailDto
        {
            To = email,
            Subject = $"Thông báo nạp tiền - {statusText}",
            Body = body,
            IsHtml = true
        });
    }

    public async Task SendBookingReminderAsync(string email, BookingConfirmationEmailDto dto)
    {
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #f59e0b;'>⏰ Nhắc nhở: Sắp đến giờ đặt sân!</h2>
                    <p>Xin chào <strong>{dto.MemberName}</strong>,</p>
                    <p>Bạn có lịch đặt sân sắp bắt đầu trong <strong style='color: #dc2626;'>1 giờ nữa</strong>:</p>
                    
                    <div style='background-color: #fffbeb; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #f59e0b;'>
                        <p><strong>🏟️ Sân:</strong> {dto.CourtName}</p>
                        <p><strong>📅 Ngày giờ:</strong> <span style='color: #dc2626; font-size: 18px; font-weight: bold;'>{dto.StartTime:dd/MM/yyyy HH:mm}</span></p>
                        <p><strong>⏱️ Thời lượng:</strong> {(dto.EndTime - dto.StartTime).TotalMinutes} phút</p>
                    </div>
                    
                    <p style='background-color: #fef3c7; padding: 10px; border-radius: 5px;'>
                        ⚠️ <strong>Lưu ý:</strong> Vui lòng đến sân trước <strong>10 phút</strong> để check-in!
                    </p>
                    
                    <p>Chúc bạn có trận đấu vui vẻ! 🎾</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #6b7280;'>
                        Email này được gửi tự động từ hệ thống Pickleball Club Management.<br>
                        Vui lòng không trả lời email này.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(new EmailDto
        {
            To = email,
            Subject = "⏰ Nhắc nhở: Sắp đến giờ đặt sân!",
            Body = body,
            IsHtml = true
        });
    }
}
