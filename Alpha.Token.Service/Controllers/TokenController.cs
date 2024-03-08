using Alpha.Common.Token;
using Alpha.Token.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Token.Controllers;

[Route("/api/token")]
public class TokenController(ITokenService tokenService, IRefreshTokenService refreshTokenService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]List<ClaimValue> claimValues)
    {
        if( claimValues == null )
            return BadRequest();

        if( !tokenService.Validate(claimValues) )
            return BadRequest();

        var token = tokenService.GenerateToken(claimValues);
        var refreshToken = await refreshTokenService.GenerateRefreshToken(token);
        var tokenString = tokenService.SerializeToken(token);

        var response = new TokenGeneration
        {
            Token = tokenString,
            RefreshToken = refreshToken.Token
        };

        return Ok(response);
    }
}
