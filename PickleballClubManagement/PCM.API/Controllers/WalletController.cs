using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Wallet;
using PCM.Application.Interfaces;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("balance/{memberId}")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetBalance(int memberId)
    {
        var result = await _walletService.GetBalanceAsync(memberId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("transactions/{memberId}")]
    public async Task<ActionResult<ApiResponse<PagedResult<WalletTransactionDto>>>> GetTransactions(int memberId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _walletService.GetTransactionsAsync(memberId, pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("deposit/{memberId}")]
    public async Task<ActionResult<ApiResponse<WalletTransactionDto>>> CreateDepositRequest(int memberId, [FromBody] DepositRequestDto dto)
    {
        var result = await _walletService.CreateDepositRequestAsync(memberId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("approve-deposit")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<bool>>> ApproveDeposit([FromBody] ApproveDepositDto dto)
    {
        var result = await _walletService.ApproveDepositAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}