using System;
using System.Security.Cryptography;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface ICouponRepository : IRepository<CouponModel>
    {
        void Update(CouponModel coupon);
    }
}

