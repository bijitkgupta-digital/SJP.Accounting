using Microsoft.AspNetCore.Mvc;

namespace SJP.Accounting.Api.Controllers;

[ApiController]
[Route("api/v1/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy" });
    }

    [HttpGet("version")]
    public IActionResult Version()
    {
        return Ok(new { Version = "1.0.0" });
    }
}