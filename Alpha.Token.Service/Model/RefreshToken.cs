using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Alpha.Token.Model;
    
[Index(nameof(Token))]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public required string Token { get; set; }
    public required string JwtId { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateExpire { get; set; }

    public required string UserId { get; set; }
}