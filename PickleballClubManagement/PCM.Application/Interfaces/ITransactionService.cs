using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Transactions;

namespace PCM.Application.Interfaces;

public interface ITransactionService
{
    Task<ApiResponse<TransactionDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResult<TransactionDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<TransactionDto>> CreateAsync(TransactionCreateDto dto, int createdBy);
    Task<ApiResponse<TransactionSummaryDto>> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Transaction Categories
    Task<ApiResponse<List<TransactionCategoryDto>>> GetCategoriesAsync();
    Task<ApiResponse<TransactionCategoryDto>> CreateCategoryAsync(TransactionCategoryCreateDto dto);
    Task<ApiResponse<TransactionCategoryDto>> UpdateCategoryAsync(int id, TransactionCategoryCreateDto dto);
    Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
}
