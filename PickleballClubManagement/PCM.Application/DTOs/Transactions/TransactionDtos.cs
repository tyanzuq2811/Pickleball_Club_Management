using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Transactions;

public class TransactionDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public TransactionType CategoryType { get; set; }
    public int? CreatedBy { get; set; }
    public string? CreatorName { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class TransactionCreateDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

public class TransactionCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}

public class TransactionCategoryCreateDto
{
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}

public class TransactionSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
    public List<CategorySummaryDto> IncomeByCategory { get; set; } = new();
    public List<CategorySummaryDto> ExpenseByCategory { get; set; } = new();
}

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
