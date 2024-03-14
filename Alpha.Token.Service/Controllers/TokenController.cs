using Alpha.Common.TokenService;
using Alpha.Token.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Token.Controllers;

[Route("/api/token")]
public class TokenController(Services.ITokenService tokenService, IRefreshTokenService refreshTokenService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]TokenRequest tokenRequest)
    {
        if( String.IsNullOrEmpty(tokenRequest.UserName) )
            return BadRequest();

        if( tokenRequest.ClaimValues == null )
            return BadRequest();

        var token = tokenService.GenerateToken(tokenRequest.UserName, tokenRequest.ClaimValues);
        var refreshToken = await refreshTokenService.GenerateRefreshToken(token.Id, tokenRequest.UserId);
        var tokenString = tokenService.SerializeToken(token);

        var response = new TokenGeneration
        {
            Token = tokenString,
            RefreshToken = refreshToken.Token
        };

        return Ok(response);
    }
}
