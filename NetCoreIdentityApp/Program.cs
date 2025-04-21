using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using NetCoreIdentityApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<NetCoreIdentityAppContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")
    )
);

// buradaki identity optionları kod kalabalığını azaltmak için extensiona taşıdık
builder.Services.AddIdentityWithExtension();

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityAppCookie";
    opt.LoginPath = new PathString("/Home/SignIn");
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
app.UseAuthorization();

// area yı modern şekilde eklemek. Area Readme sodyasında eski versionunki kalmış
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();