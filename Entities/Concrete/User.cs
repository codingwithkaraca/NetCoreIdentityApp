using Microsoft.AspNetCore.Identity;

namespace Entities.Concrete;

public class User : IdentityUser
{
    public string? City { get; set; }
    public string? Picture { get; set; }
    public DateTime? BirthDate { get; set; }
    public byte? Gender { get; set; }
}