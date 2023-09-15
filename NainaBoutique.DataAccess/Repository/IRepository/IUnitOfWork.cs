using System;
namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
        IProductRepository Product { get; }
		IUserRepository User { get; }
		ICartRepository Cart { get; }
		ICouponRepository Coupon { get; }
		IFavouritesRepository Favourite { get; }
		IGiftcardRepository Giftcard { get; }
		IProductImageRepository ProductImage { get; }
        void Save();

		

	}
}

