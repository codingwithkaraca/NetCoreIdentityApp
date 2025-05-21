using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.OptionModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NetCoreIdentityApp.ClaimProviders;
using NetCoreIdentityApp.Extensions;
using NetCoreIdentityApp.Requirements;
using NetCoreIdentityApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<NetCoreIdentityAppContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")
    )
);

// 30 dakikada bir kontrol -> önemli bilgiler(kullanıcı bilgileri vb.) değişmiş mi kontrol etmek için
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(20);
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// buradaki identity optionları kod kalabalığını azaltmak için extensiona taşıdık
builder.Services.AddIdentityWithExtension();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city","ankara");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
});

// yaşa göre şiddet sayfasını yetkilendirmek için
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement(){ThresholdAge = 18});
    });    
});

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityAppCookie";
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/LogOut");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // 60 günlük
    opt.SlidingExpiration = true; // 60 gün içinde 1 gün bile girse tekrardan 0 dan üzerine 60 gün ekle
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// area yı modern şekilde eklemek. Area Readme sodyasında eski versionunki kalmış
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();