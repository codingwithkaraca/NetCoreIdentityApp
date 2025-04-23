using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels;

public class ForgetPasswordVM
{
    [Required(ErrorMessage = "Email alanı boş geçilemez")]
    [EmailAddress(ErrorMessage = "Geçerli email formatı giriniz")]
    [Display(Name = "Email")]
    public string Email { get; set; }
}