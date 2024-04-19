using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Alpha.Common.TokenService;
using Alpha.Tools.Security;
using Microsoft.IdentityModel.Tokens;

namespace Alpha.Token.Services;

public interface ITokenGenerationService
{
    JwtSecurityToken GenerateToken(string userName, List<ClaimValue> claimValues);

    string SerializeToken(JwtSecurityToken token);
}

public class TokenGenerationSevice(JwtOptions jwtOptions) : ITokenGenerationService
{
    public JwtSecurityToken GenerateToken(string userName, List<ClaimValue> claimValues)
    {
        List<Claim> claims = [ 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, userName)
        ];
        claims.AddRange(claimValues.Select(x => new Claim(x.Type!, x.Value!)));

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