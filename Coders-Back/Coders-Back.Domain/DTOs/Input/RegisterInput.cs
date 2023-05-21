using System.ComponentModel.DataAnnotations;

namespace Coders_Back.Domain.DTOs.Input;

public class RegisterInput
{
    public RegisterInput(string email, string name, string password, string passwordConfirm)
    {
        Email = email;
        Name = name;
        Password = password;
        PasswordConfirm = passwordConfirm;
    }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters long", MinimumLength = 6)]
    public string Password { get; set; }
    
    [Compare(nameof(Password), ErrorMessage = "Password fields must match")]
    public string PasswordConfirm { get; set; }
}