using System.ComponentModel.DataAnnotations;

namespace Coders_Back.Domain.DTOs.Input;

public class UserInput
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "BirthDate is required")]
    public DateTime BirthDate { get; set; }
    
    // TODO: make only numbers
    public string? PhoneNumber { get; set; }
    
    public string? GithubProfile { get; set; }
    
    public Guid AddressId { get; set; }
    
    public string? LinkedinUrl { get; set; }
}