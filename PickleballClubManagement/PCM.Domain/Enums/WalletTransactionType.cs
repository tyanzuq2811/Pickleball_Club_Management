namespace PCM.Domain.Enums;

public enum WalletTransactionType
{
    Deposit = 0,        // Nạp tiền
    PayBooking = 1,     // Trả tiền đặt sân
    ReceivePrize = 2,   // Nhận thưởng
    Refund = 3,         // Hoàn tiền
    PayEntryFee = 4     // Đóng phí giải đấu
}
