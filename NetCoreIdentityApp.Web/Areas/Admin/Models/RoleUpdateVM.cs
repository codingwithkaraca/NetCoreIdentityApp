using System.ComponentModel.DataAnnotations;

namespace NetCoreIdentityApp.Web.Areas.Admin.Models;

public class RoleUpdateVM
{
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Rol isim alanı boş bırakılamaz")]
    [Display(Name = "İsim")]
    public string Name { get; set; }
}