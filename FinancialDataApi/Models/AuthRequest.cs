using System.ComponentModel.DataAnnotations;

namespace FinancialDataApi.Models;

public class AuthenticationRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
