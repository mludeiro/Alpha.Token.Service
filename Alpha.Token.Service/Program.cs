using System.Text;
using Alpha.Common.Consul;
using Alpha.Common.Database;
using Alpha.Token.Configuration;
using Alpha.Token.Data;
using Alpha.Token.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        builder.Services.AddControllers();
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

        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
        builder.Services.AddSingleton(jwtOptions);
        
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key!)),

            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        builder.Services.AddSingleton(tokenValidationParameters);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;
        });

        builder.Services.ConsulServicesConfig(builder.Configuration.GetSection("Consul").Get<ConsulConfig>()!);

        builder.Services.AddScoped<ITokenService,TokenSevice>();
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

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }

}

