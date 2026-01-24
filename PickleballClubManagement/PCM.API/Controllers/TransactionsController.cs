using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Transactions;
using PCM.Application.Interfaces;

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
    public async Task<ActionResult<ApiResponse<TransactionDto>>> GetTransactionById(int id)
    {
        var result = await _transactionService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TransactionDto>>>> GetAllTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _transactionService.GetAllAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> CreateTransaction([FromBody] TransactionCreateDto dto)
    {
        var createdBy = int.Parse(User.FindFirst("Id")?.Value ?? "0");
        var result = await _transactionService.CreateAsync(dto, createdBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<TransactionSummaryDto>>> GetTransactionSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _transactionService.GetSummaryAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}