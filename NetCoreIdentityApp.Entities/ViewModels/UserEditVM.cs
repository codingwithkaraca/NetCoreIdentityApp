using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NetCoreIdentityApp.Entities.ViewModels;

public class UserEditVM
{
    
    [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz")]
    [Display(Name = "Kullanıcı adı")]
    public string UserName { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Geçerli email formatı giriniz")]
    [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
    [Display(Name = "Telefon")]
    public string Phone { get; set; }  = null!;
    
    [Display(Name = "Doğum tarihi")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }
    
    [Display(Name = "Şehir")]
    public string? City { get; set; }
    
    [Display(Name = "Cinsiyet")]
    public byte? Gender { get; set; }
    
    [Display(Name = "Profil resmi")]
    public IFormFile? Picture { get; set; }
    
    public string? PictureUrl { get; set; }
    
    
}