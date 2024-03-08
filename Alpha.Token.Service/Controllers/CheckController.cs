using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Token.Controllers;

[Route("/api/check")]
public class CheckController : ControllerBase
{
    // Endpoint for validating jwt signature
    [HttpGet]
    [Authorize]
    public OkResult Get()
    {
        return Ok();
    }
}
