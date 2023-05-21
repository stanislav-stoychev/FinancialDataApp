using FinancialDataApi.Data;
using FinancialDataApi.Models;
using FinancialDataApi.Services;
using Microsoft.AspNetCore.Identity;

namespace FinancialDataApi.Endpoints;

public static class AuthEndpoints
{
    public static async Task<IResult> Register(
        RegistrationRequest request,
        UserManager<IdentityUser> userManager
    )
    {
        var result = await userManager.CreateAsync(
            new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email
            },
            request.Password
        );

        if (result.Succeeded)
        {
            request.Password = string.Empty;
            return Results.Created("/register", new RegistrationResponse { Email = request.Email });
        }

        return Results.BadRequest(result.Errors);
    }

    public static async Task<IResult> Login(
        AuthenticationRequest request,
        UserManager<IdentityUser> userManager,
        ApiDatabaseContext context,
        TokenService tokenService,
        CancellationToken token
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Results.BadRequest("Bad credentials");

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Results.BadRequest("Bad credentials");

        var userInDb = context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (userInDb is null)
            return Results.Unauthorized();

        var accessToken = tokenService.CreateToken(userInDb);
        await context.SaveChangesAsync(token);

        return Results.Ok(
            new AuthenticationResponse
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken,
            }
        );
    }
}
