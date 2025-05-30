

using System.ComponentModel.DataAnnotations;

namespace NetCoreIdentityApp.Web.Areas.Admin.Models;

public class RoleCreateVM
{
    [Required(ErrorMessage = "Rol isim alanı bırakılamaz")]
    [Display(Name = "Rol ismi")]
    public string Name { get; set; }
}