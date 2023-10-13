
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
   
    public DbSet<CouponModel> Coupons { get; set; }
    public DbSet<FavouritesModel> Favourites { get; set; }
    public DbSet<GiftcardModel> Giftcards { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<OrderSummary> OrderSummaries { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    
    public DbSet<AppliedCoupon> AppliedCoupons { get; set; }
    public DbSet<WalletModel> WalletModels { get; set; }

    public DbSet<AddressModel> Address { get; set; }
    public DbSet<WalletTopUp> WalletTopUps { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Entity<CategoryModel>().HasData(

        //    new CategoryModel { Id = 10, CategoryName = "Party Dresses", Description = "Get your Hands on most exquite Party Dresses Collection from our store made with Love",RecStatus ='A', CreatedAt = DateTime.Now  },
        //    new CategoryModel { Id = 11, CategoryName = "Bows", Description = "Get your Hands on most exquite Bows Collection from our store made with Love" , RecStatus = 'A', CreatedAt = DateTime.Now },
        //    new CategoryModel { Id = 12, CategoryName = "Rompers", Description = "Get your Hands on most exquite Rompers Collection from our store made with Love", RecStatus = 'A', CreatedAt = DateTime.Now }

        //    );

        //modelBuilder.Entity<ProductModel>().HasData(
        //    new ProductModel
        //    {
        //        Id = 11,
        //        ProductName = "Star Ruffle Party Dress Black",
        //        Description = "Beautiful Party wear dress with Self Tie Star Bow Ruffle Party Dress Black perfect for every occasion",
        //        Price = 150,
        //        Sale_Price = 135,
        //        QuantityInStock = 10,
        //        Color = "Black",
        //        Size = "3-4yr",
        //        CategoryId = 1,
        //        RecStatus = 'A',
        //        CreatedAt = DateTime.Now
        //    },

        //    new ProductModel
        //    {
        //        Id = 12,
        //        ProductName = "Korean Smocked Sparkle Dress",
        //        Description = "Beautiful flowy party wear dress perfection for summer.",
        //        Price = 240,
        //        Sale_Price = 216,
        //        QuantityInStock = 15,
        //        Color = "Biege",
        //        Size = "3-4yr",
        //        CategoryId = 1,
        //        RecStatus = 'A',
            //    CreatedAt = DateTime.Now
            //});




    }

}


