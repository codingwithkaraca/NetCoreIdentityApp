namespace NetCoreIdentityApp.Web.Areas.Admin.Models;

public class AssignRoleToUserVM
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsExists { get; set; }
    
}