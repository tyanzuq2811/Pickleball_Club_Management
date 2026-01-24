using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Courts;
using PCM.Application.Interfaces;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourtsController : ControllerBase
{
    private readonly ICourtService _courtService;

    public CourtsController(ICourtService courtService)
    {
        _courtService = courtService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CourtDto>>>> GetAll()
    {
        var result = await _courtService.GetAllAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CourtDto>>> GetById(int id)
    {
        var result = await _courtService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CourtDto>>> Create([FromBody] CourtCreateDto dto)
    {
        var result = await _courtService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CourtDto>>> Update(int id, [FromBody] CourtUpdateDto dto)
    {
        var result = await _courtService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var result = await _courtService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}