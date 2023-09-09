using System;
using System.Linq.Expressions;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
	public class CartRepository : Repository<ShoppingCart> , ICartRepository
	{

        private ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart cart)
        {
            _db.Carts.Update(cart);
        }
    }
}

