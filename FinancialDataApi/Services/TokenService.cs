using FinancialDataApi.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinancialDataApi.Services;

public class TokenService
{
    private readonly FinancialDataApiOptions _options;

    public TokenService(
        FinancialDataApiOptions options
    ) => _options = options;

    public string CreateToken(
        IdentityUser user
    )
    {
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            CreateClaims(user),
            expires: DateTime.UtcNow.AddMinutes(_options.TokenExpirationTimeInMinutes),
            signingCredentials: CreateSigningCredentials()
        );
        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private static List<Claim> CreateClaims(
        IdentityUser user
    )
    =>  new()
    {
        new Claim(JwtRegisteredClaimNames.Sub, "TokenForFinancialDataApi"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

    private SigningCredentials CreateSigningCredentials()
    => new(
        new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey)
        ),
        SecurityAlgorithms.HmacSha256
    );
}