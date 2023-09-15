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

        public IUserRepository User { get; private set; }

        public ICartRepository Cart { get; private set; }

        public ICouponRepository Coupon { get; private set; }

        public IFavouritesRepository Favourite { get; private set; }

        public IGiftcardRepository Giftcard { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            User = new UserRepository(_db);
            Cart = new CartRepository(_db);
            Coupon = new CouponRepository(_db);
            Favourite = new FavouritesRepository(_db);
            Giftcard = new GiftcardRepository(_db);
            ProductImage = new ProductImageRepository(_db);
        }
        
        public void Save()
        {
            _db.SaveChanges();
        }

        
    }
}

