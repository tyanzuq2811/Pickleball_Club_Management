using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Transactions;
using PCM.Application.Interfaces;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/transactioncategories")]
[Authorize(Roles = "Treasurer")]
public class TransactionCategoriesController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionCategoriesController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TransactionCategoryDto>>>> GetAllCategories()
    {
        var result = await _transactionService.GetCategoriesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransactionCategoryDto>>> CreateCategory([FromBody] TransactionCategoryCreateDto dto)
    {
        var result = await _transactionService.CreateCategoryAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TransactionCategoryDto>>> UpdateCategory(int id, [FromBody] TransactionCategoryCreateDto dto)
    {
        var result = await _transactionService.UpdateCategoryAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
    {
        var result = await _transactionService.DeleteCategoryAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
