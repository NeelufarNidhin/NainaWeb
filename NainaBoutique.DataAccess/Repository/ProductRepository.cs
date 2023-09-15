using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class ProductRepository : Repository<ProductModel>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductModel product)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            if(objFromDb != null)
            {
                objFromDb.ProductName = product.ProductName;
                objFromDb.Description = product.Description;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.Size = product.Size;
                objFromDb.Price = product.Price;
                objFromDb.Sale_Price = product.Sale_Price;
                objFromDb.QuantityInStock = product.QuantityInStock;
                objFromDb.Color = product.Color;
                objFromDb.ProductImage = product.ProductImage;
                //if(product.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = product.ImageUrl;

                //}
            }
        }
    }
}

