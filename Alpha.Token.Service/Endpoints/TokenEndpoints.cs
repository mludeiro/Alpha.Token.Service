using Alpha.Common.TokenService;
using Alpha.Token.Services;

namespace Alpha.Token.Endpoints;

internal static class EndpointsMappings
{
    public static void MapAlphaTokenEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet( "/", () => "Token Server");

        app.MapGet("/api/check", () => "").RequireAuthorization();

        app.MapPost("/api/token", GenerateToken );
    }

    public static async Task<IResult> GenerateToken(TokenRequest tokenRequest, 
        ITokenGenerationService tokenService, IRefreshTokenService refreshTokenService)
    {
        if( string.IsNullOrEmpty(tokenRequest.UserName) )
            return Results.BadRequest();

        if( tokenRequest.ClaimValues == null )
            return Results.BadRequest();

        var token = tokenService.GenerateToken(tokenRequest.UserName, tokenRequest.ClaimValues);
        var refreshToken = await refreshTokenService.GenerateRefreshToken(token.Id, tokenRequest.UserId);
        var tokenString = tokenService.SerializeToken(token);

        var response = new TokenGeneration
        {
            Token = tokenString,
            RefreshToken = refreshToken.Token
        };

        return Results.Ok(response);
    }
}