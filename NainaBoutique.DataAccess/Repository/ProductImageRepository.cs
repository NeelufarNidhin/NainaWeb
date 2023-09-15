using System;
using System.Linq.Expressions;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(ProductImage productImage)
        {
            _db.ProductImages.Update(productImage);
        }
    }
}

