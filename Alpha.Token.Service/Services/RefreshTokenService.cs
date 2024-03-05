using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Alpha.Token.Data;
using Alpha.Token.Model;

namespace Alpha.Token.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshToken( JwtSecurityToken token );
}

public class RefreshTokenService(TokenDataContext dataContext) : IRefreshTokenService
{
    public async Task<RefreshToken> GenerateRefreshToken( JwtSecurityToken token )
    {
        var userId = token.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsRevoked = false,
            UserId = userId!,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(6),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        await dataContext.RefreshTokens.AddAsync(refreshToken);
        await dataContext.SaveChangesAsync();

        return refreshToken;
    }
}