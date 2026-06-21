using alposim.Interfaces;
using alposim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SyncController : ControllerBase
{
    private readonly ISyncRepository _syncRepository;

    public SyncController(ISyncRepository syncRepository)
    {
        _syncRepository = syncRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Sync>))]
    public async Task<IActionResult> GetAll()
    {
        var syncs = await _syncRepository.GetAll();
        return Ok(syncs);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(200, Type = typeof(Sync))]
    public async Task<IActionResult> GetSyncById(Guid id)
    {
        var sync = await _syncRepository.GetSyncById(id);
        if (sync == null) return NotFound();
        return Ok(sync);
    }

    [HttpGet("{id}/status")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetSyncStatus(Guid id)
    {
        var status = await _syncRepository.GetSyncStatus(id);
        if (status == null) return NotFound();
        return Ok(status);
    }

    [HttpGet("range")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Sync>))]
    public async Task<IActionResult> GetSyncByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var syncs = await _syncRepository.GetSyncByDate(startDate, endDate);
        return Ok(syncs);
    }

    [HttpPost("start")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(200, Type = typeof(Sync))]
    public async Task<IActionResult> StartSync()
    {
        var sync = await _syncRepository.StartSync();
        return Ok(sync);
    }

    [HttpPost("stop")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(200, Type = typeof(Sync))]
    public async Task<IActionResult> StopSync()
    {
        var sync = await _syncRepository.StopSync();
        if (sync == null) return NotFound("No active sync found.");
        return Ok(sync);
    }
    [HttpPost("pull")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200, Type = typeof(Sync))]
    public async Task<IActionResult> PullSync()
    {
        var sync = await _syncRepository.PullSync();
        if (sync == null) return BadRequest("Pull Sync Error");
        return Ok(sync);
    }
}