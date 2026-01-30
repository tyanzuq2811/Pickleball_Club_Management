using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Transactions;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;

namespace PCM.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<TransactionDto>> GetByIdAsync(int id)
    {
        var t = await _unitOfWork.Transactions.GetByIdAsync(id);
        if (t == null) return ApiResponse<TransactionDto>.ErrorResponse("Transaction not found");
        var dto = new TransactionDto { Id = t.Id, Date = t.Date, Amount = t.Amount, Description = t.Description, CategoryId = t.CategoryId, CreatedBy = t.CreatedBy };
        return ApiResponse<TransactionDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<PagedResult<TransactionDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var list = await _unitOfWork.Transactions.GetAllAsync();
        var total = list.Count();
        var items = list.OrderByDescending(x => x.Date).Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(t => new TransactionDto { Id = t.Id, Date = t.Date, Amount = t.Amount, Description = t.Description, CategoryId = t.CategoryId, CreatedBy = t.CreatedBy }).ToList();

        var result = new PagedResult<TransactionDto> { Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
        return ApiResponse<PagedResult<TransactionDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<TransactionDto>> CreateAsync(TransactionCreateDto dto, int createdBy)
    {
        var t = new Transaction { Date = dto.Date, Amount = dto.Amount, Description = dto.Description, CategoryId = dto.CategoryId, CreatedBy = createdBy, CreatedDate = DateTime.UtcNow };
        await _unitOfWork.Transactions.AddAsync(t);
        await _unitOfWork.SaveChangesAsync();
        var res = new TransactionDto { Id = t.Id, Date = t.Date, Amount = t.Amount, Description = t.Description, CategoryId = t.CategoryId, CreatedBy = t.CreatedBy };
        return ApiResponse<TransactionDto>.SuccessResponse(res, "Transaction created");
    }

    public async Task<ApiResponse<TransactionDto>> UpdateAsync(int id, TransactionCreateDto dto)
    {
        var t = await _unitOfWork.Transactions.GetByIdAsync(id);
        if (t == null) return ApiResponse<TransactionDto>.ErrorResponse("Transaction not found");
        
        t.Date = dto.Date;
        t.Amount = dto.Amount;
        t.Description = dto.Description;
        t.CategoryId = dto.CategoryId;
        
        _unitOfWork.Transactions.Update(t);
        await _unitOfWork.SaveChangesAsync();
        
        var res = new TransactionDto { Id = t.Id, Date = t.Date, Amount = t.Amount, Description = t.Description, CategoryId = t.CategoryId, CreatedBy = t.CreatedBy };
        return ApiResponse<TransactionDto>.SuccessResponse(res, "Transaction updated");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var t = await _unitOfWork.Transactions.GetByIdAsync(id);
        if (t == null) return ApiResponse<bool>.ErrorResponse("Transaction not found");
        
        _unitOfWork.Transactions.Remove(t);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.SuccessResponse(true, "Transaction deleted");
    }

    public async Task<ApiResponse<TransactionSummaryDto>> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var list = await _unitOfWork.Transactions.GetAllAsync();
        var filtered = list.AsQueryable();
        if (startDate.HasValue) filtered = filtered.Where(t => t.Date >= startDate.Value);
        if (endDate.HasValue) filtered = filtered.Where(t => t.Date <= endDate.Value);

        var totalIncome = filtered.Where(t => t.Amount > 0).Sum(t => t.Amount);
        var totalExpense = filtered.Where(t => t.Amount < 0).Sum(t => t.Amount);

        var dto = new TransactionSummaryDto { TotalIncome = totalIncome, TotalExpense = totalExpense };
        return ApiResponse<TransactionSummaryDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<List<TransactionCategoryDto>>> GetCategoriesAsync()
    {
        var list = await _unitOfWork.TransactionCategories.GetAllAsync();
        var dto = list.Select(c => new TransactionCategoryDto { Id = c.Id, Name = c.Name, Type = c.Type }).ToList();
        return ApiResponse<List<TransactionCategoryDto>>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<TransactionCategoryDto>> CreateCategoryAsync(TransactionCategoryCreateDto dto)
    {
        var cat = new TransactionCategory { Name = dto.Name, Type = dto.Type, CreatedDate = DateTime.UtcNow };
        await _unitOfWork.TransactionCategories.AddAsync(cat);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<TransactionCategoryDto>.SuccessResponse(new TransactionCategoryDto { Id = cat.Id, Name = cat.Name, Type = cat.Type }, "Category created");
    }

    public async Task<ApiResponse<TransactionCategoryDto>> UpdateCategoryAsync(int id, TransactionCategoryCreateDto dto)
    {
        var cat = await _unitOfWork.TransactionCategories.GetByIdAsync(id);
        if (cat == null) return ApiResponse<TransactionCategoryDto>.ErrorResponse("Category not found");
        
        cat.Name = dto.Name;
        cat.Type = dto.Type;
        
        _unitOfWork.TransactionCategories.Update(cat);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<TransactionCategoryDto>.SuccessResponse(new TransactionCategoryDto { Id = cat.Id, Name = cat.Name, Type = cat.Type }, "Category updated");
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
    {
        var cat = await _unitOfWork.TransactionCategories.GetByIdAsync(id);
        if (cat == null) return ApiResponse<bool>.ErrorResponse("Category not found");
        _unitOfWork.TransactionCategories.Remove(cat);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Category deleted");
    }

    public async Task<byte[]> ExportToExcelAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        // TODO: Implement Excel export using EPPlus or ClosedXML
        // For now, return dummy data
        var list = await _unitOfWork.Transactions.GetAllAsync();
        var filtered = list.AsQueryable();
        if (startDate.HasValue) filtered = filtered.Where(t => t.Date >= startDate.Value);
        if (endDate.HasValue) filtered = filtered.Where(t => t.Date <= endDate.Value);
        
        // Return empty byte array as placeholder
        return System.Text.Encoding.UTF8.GetBytes("Excel export not implemented yet");
    }

    public async Task<byte[]> ExportToPdfAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        // TODO: Implement PDF export using iTextSharp or similar
        // For now, return dummy data
        var list = await _unitOfWork.Transactions.GetAllAsync();
        var filtered = list.AsQueryable();
        if (startDate.HasValue) filtered = filtered.Where(t => t.Date >= startDate.Value);
        if (endDate.HasValue) filtered = filtered.Where(t => t.Date <= endDate.Value);
        
        // Return empty byte array as placeholder
        return System.Text.Encoding.UTF8.GetBytes("PDF export not implemented yet");
    }
}
