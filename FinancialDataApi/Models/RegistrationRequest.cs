using System.ComponentModel.DataAnnotations;

namespace FinancialDataApi.Models;

public class RegistrationRequest
{
    public string Email { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}