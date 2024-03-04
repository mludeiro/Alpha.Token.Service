using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Alpha.Common.Token;
using Alpha.Token.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Alpha.Token.Services;

public interface ITokenService
{
    JwtSecurityToken GenerateToken(List<ClaimValue> claimValues);
    string SerializeToken(JwtSecurityToken token);
}

public class TokenSevice(JwtOptions jwtOptions) : ITokenService
{
    public JwtSecurityToken GenerateToken(List<ClaimValue> claimValues)
    {
        var claims = claimValues.Select(x => new Claim(x.Type!, x.Value!));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return token;
    }

    public string SerializeToken(JwtSecurityToken token) => new JwtSecurityTokenHandler().WriteToken(token);

}