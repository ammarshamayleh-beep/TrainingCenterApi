using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Services;

public interface ITokenService
{
    string CreateToken(User user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(User user)
    {
        // 1. The claims: what we write about the user inside the token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // 2. The secret key, to sign the token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        // 3. Everything the token needs
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresInMinutes")),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        // 4. Build the token string: header.payload.signature
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}