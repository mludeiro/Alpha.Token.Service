using Microsoft.AspNetCore.Mvc;

namespace Alpha.Token.Controllers;

[ApiExplorerSettings(IgnoreApi=true)]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpGet]
    public ObjectResult Get()
    {
        return Ok("Token Server");
    }
}