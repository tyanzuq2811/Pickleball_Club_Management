using PCM.Application.DTOs.Email;

namespace PCM.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailDto emailDto);
    Task SendBookingConfirmationAsync(string email, BookingConfirmationEmailDto dto);
    Task SendPaymentSuccessAsync(string email, PaymentSuccessEmailDto dto);
    Task SendWalletDepositNotificationAsync(string email, WalletDepositEmailDto dto);
    Task SendBookingReminderAsync(string email, BookingConfirmationEmailDto dto);
}
