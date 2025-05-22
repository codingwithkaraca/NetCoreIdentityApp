using System.Text;
using NetCoreIdentityApp.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NetCoreIdentityApp.Web.TagHelpers;

public class UserRoleNamesTagHelper : TagHelper
{
    public string UserId { get; set; } = null!;
    private readonly UserManager<User> _userManager;
    public UserRoleNamesTagHelper(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        var userRole = await _userManager.GetRolesAsync(user!);
        StringBuilder stringBuilder = new StringBuilder();
        userRole.ToList().ForEach(item =>
        {
            stringBuilder.Append(@$"<span class='badge bg-secondary mx-1'>{item.ToLower()}</span>");
        });
        output.Content.SetHtmlContent(stringBuilder.ToString());
    }
    
}