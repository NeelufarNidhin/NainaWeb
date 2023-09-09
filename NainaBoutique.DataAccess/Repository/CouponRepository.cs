using System;
using System.Linq.Expressions;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class CouponRepository : Repository<CouponModel>, ICouponRepository
    {
       
         private ApplicationDbContext _db;
        public CouponRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(CouponModel coupon)
        {
            _db.Coupons.Update(coupon);
        }
    }
}

