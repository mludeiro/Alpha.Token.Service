using Microsoft.AspNetCore.Mvc;

namespace Alpha.Token.Controllers;

[Route("/api/token")]
public class TokenController : ControllerBase
{
    [HttpPost]
    public ObjectResult Post()
    {
        return Ok("Token Server");
    }
}
