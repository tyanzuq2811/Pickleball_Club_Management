using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Transactions;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Treasurer")]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> GetTransactionById(int id)
    {
        var result = await _transactionService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    [Authorize(Roles = "Treasurer")]
    public async Task<ActionResult<ApiResponse<PagedResult<TransactionDto>>>> GetAllTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _transactionService.GetAllAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [Authorize(Roles = "Treasurer")]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> CreateTransaction([FromBody] TransactionCreateDto dto)
    {
        var createdBy = int.Parse(User.FindFirst("Id")?.Value ?? "0");
        var result = await _transactionService.CreateAsync(dto, createdBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Treasurer")]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> UpdateTransaction(int id, [FromBody] TransactionCreateDto dto)
    {
        var result = await _transactionService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Treasurer")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTransaction(int id)
    {
        var result = await _transactionService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<TransactionSummaryDto>>> GetTransactionSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _transactionService.GetSummaryAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("export")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<IActionResult> ExportTransactions(
        [FromQuery] string format = "excel",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (format.ToLower() == "excel")
        {
            var excelData = await _transactionService.ExportToExcelAsync(startDate, endDate);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Transactions_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        else if (format.ToLower() == "pdf")
        {
            var pdfData = await _transactionService.ExportToPdfAsync(startDate, endDate);
            return File(pdfData, "application/pdf", $"Transactions_{DateTime.Now:yyyyMMdd}.pdf");
        }

        return BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Format không hợp lệ. Sử dụng 'excel' hoặc 'pdf'"
        });
    }
}