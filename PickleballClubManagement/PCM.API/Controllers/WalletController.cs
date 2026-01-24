using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Wallet;
using PCM.Application.Interfaces;
using System.Security.Claims;

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

    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<List<WalletTransactionDto>>>> GetMyTransactions()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        
        var result = await _walletService.GetMemberTransactionsAsync(userId);
        return Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<ActionResult<ApiResponse<bool>>> RequestDeposit([FromBody] DepositRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _walletService.RequestDepositAsync(userId, request.Amount);
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<List<WalletTransactionDto>>>> GetPendingDeposits()
    {
        var result = await _walletService.GetPendingDepositsAsync();
        return Ok(result);
    }

    [HttpPost("approve/{id}")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<bool>>> ApproveDeposit(int id)
    {
        var result = await _walletService.ApproveDepositAsync(id);
        return Ok(result);
    }
}