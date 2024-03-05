using Alpha.Token.Model;
using Microsoft.EntityFrameworkCore;

namespace Alpha.Token.Data;

public class TokenDataContext(DbContextOptions<TokenDataContext> options) : DbContext(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}