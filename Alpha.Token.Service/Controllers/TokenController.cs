using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Alpha.Common.Token;
using Alpha.Token.Configuration;
using Alpha.Token.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Alpha.Token.Controllers;

[Route("/api/token")]
public class TokenController(ITokenService tokenService) : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody]List<ClaimValue> claimValues)
    {
        if( claimValues == null )
            return BadRequest();

        var token = tokenService.GenerateToken(claimValues);
        var tokenString = tokenService.SerializeToken(token);

        var response = new TokenGeneration
        {
            Token = tokenString
        };

        return Ok(response);
    }
}
