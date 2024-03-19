using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Alpha.Token.Data;
using Alpha.Token.Model;
using Microsoft.EntityFrameworkCore;

namespace Alpha.Token.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshToken( string tokenId, string userId );
}

public class RefreshTokenService(TokenDataContext dataContext) : IRefreshTokenService
{
    public async Task<RefreshToken> GenerateRefreshToken( string tokenId, string userId )
    {
        var refreshToken = new RefreshToken()
        {
            JwtId = tokenId,
            IsRevoked = false,
            UserId = userId,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(6),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        
        // Only one RT per user
        var oldRt = dataContext.RefreshTokens.Where(rt => rt.UserId == userId);
        dataContext.RefreshTokens.RemoveRange(oldRt);

        await dataContext.RefreshTokens.AddAsync(refreshToken);
        await dataContext.SaveChangesAsync();

        return refreshToken;
    }
}