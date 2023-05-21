using System.ComponentModel.DataAnnotations;

namespace FinancialDataApi.Models;

public class AuthenticationResponse
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }
}