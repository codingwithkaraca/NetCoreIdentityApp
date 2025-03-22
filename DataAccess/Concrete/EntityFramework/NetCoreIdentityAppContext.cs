using Entities.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework;

public class NetCoreIdentityAppContext : IdentityDbContext<User, UserRole, string>
{
    public NetCoreIdentityAppContext(DbContextOptions<NetCoreIdentityAppContext> options) : base(options) { }
    
    
    
    
}