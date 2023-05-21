using System.ComponentModel.DataAnnotations;
using Coders_Back.Domain.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Coders_Back.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, ISoftDelete   
{
    public override Guid Id { get; set; }
    // The UserName from IdentityUser is always the same as Email, we must use the Name to handle the username
    [Required]
    public string? Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string? GithubProfile { get; set; }
    public Guid AddressId { get; set; }
    public string? LinkedinUrl { get; set; }
    public bool IsDeleted { get; set; }
}