using System.ComponentModel.DataAnnotations;

namespace Coders_Back.Domain.DTOs.Input;

public class LoginInput
{
    public LoginInput(string identifier, string password)
    {
        Identifier = identifier;
        Password = password;
    }

    [Required(ErrorMessage = "Email or username is required")]
    public string Identifier { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}