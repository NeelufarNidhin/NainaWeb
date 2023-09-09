using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public ApplicationDbContext()
    {

    }


    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ShoppingCart> Carts { get; set; }
     public DbSet<SizeModel> Sizes { get; set; }
    public DbSet<CouponModel> Coupons { get; set; }


}

