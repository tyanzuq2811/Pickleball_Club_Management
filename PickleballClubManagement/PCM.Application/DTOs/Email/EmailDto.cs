namespace PCM.Application.DTOs.Email;

public class EmailDto
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}

public class BookingConfirmationEmailDto
{
    public string MemberName { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingId { get; set; } = string.Empty;
}

public class PaymentSuccessEmailDto
{
    public string MemberName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class WalletDepositEmailDto
{
    public string MemberName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DepositDate { get; set; }
}
