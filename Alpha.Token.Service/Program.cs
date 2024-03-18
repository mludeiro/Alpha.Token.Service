using Alpha.Common.Consul;
using Alpha.Common.Database;
using Alpha.Common.Security;
using Alpha.Token.Data;
using Alpha.Token.Endpoints;
using Alpha.Token.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Alpha.Token;

internal class Program
{
    private static void Main(string[] args)
    {
        Run(Build(args));
    }

    private static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHealthChecks();

        builder.Services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            o.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        builder.Services.AddDbContext<TokenDataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!));
        builder.Services.AddHostedService<DbMigrationBackgroundService<TokenDataContext>>();

        builder.Services.AddEndpointsApiExplorer();

        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
        builder.Services.AddSingleton(jwtOptions);
        builder.Services.AddAlphaAuthentication(jwtOptions);

        builder.Services.AddAuthorization();

        builder.Services.ConsulServicesConfig(builder.Configuration.GetSection("Consul").Get<ConsulConfig>()!);

        builder.Services.AddScoped<ITokenGenerationService,TokenGenerationSevice>();
        builder.Services.AddScoped<IRefreshTokenService,RefreshTokenService>();

        return builder.Build();
    }


    private static void Run(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Token Service"));
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAlphaTokenEndpoints();
//        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }

}

