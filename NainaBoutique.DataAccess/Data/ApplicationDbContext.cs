
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext

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
    public DbSet<FavouritesModel> Favourites { get; set; }
    public DbSet<GiftcardModel> Giftcards { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<OrderSummary> OrderSummaries { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<OtpModel> OtpModels { get; set; }
    public DbSet<AppliedCoupon> AppliedCoupons { get; set; }
    public DbSet<WalletModel> WalletModels { get; set; }
    public DbSet<AddressModel> Address { get; set; }


    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Deleted && entity is ISoftDelete)
            {
                entry.State = EntityState.Modified;
                entity.GetType().GetProperty("RecStatus").SetValue(entity, 'D');

            }
        }

        return base.SaveChanges();
    }

    public void UpdateOTP(string email, string otpdb)
    {
        var otpFromDb = new OtpModel()
        {
            Email=email,
            Otp=otpdb
        };

        
        
    }

}

