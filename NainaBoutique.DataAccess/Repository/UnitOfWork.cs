using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork

    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category  { get; private set;}

        public IProductRepository Product { get; private set; }

        public IUserRepository ApplicationUser { get; private set; }

        public ICartRepository Cart { get; private set; }

        public ICouponRepository Coupon { get; private set; }

        public IFavouritesRepository Favourite { get; private set; }

        public IGiftcardRepository Giftcard { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public IOrderSummaryRepository OrderSummary { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }

        public IWalletRepository Wallet { get; private set; }
        public IWalletTopUpRepository WalletTopUp { get; private set; }
        public IAddressRepository Address { get; private set; }


        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            ApplicationUser = new UserRepository(_db);
            Cart = new CartRepository(_db);
            Coupon = new CouponRepository(_db);
            Favourite = new FavouritesRepository(_db);
            Giftcard = new GiftcardRepository(_db);
            ProductImage = new ProductImageRepository(_db);
            OrderSummary = new OrderSummaryRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Wallet = new WalletRepository(_db);
            WalletTopUp = new WalletTopUpRepository(_db);
            Address = new AddressRepository(_db);
        }
        
        public void Save()
        {
            _db.SaveChanges();
        }

        
    }
}

