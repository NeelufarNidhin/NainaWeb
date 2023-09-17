using System;
namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
        IProductRepository Product { get; }
		IUserRepository ApplicationUser { get; }
		ICartRepository Cart { get; }
		ICouponRepository Coupon { get; }
		IFavouritesRepository Favourite { get; }
		IGiftcardRepository Giftcard { get; }
		IProductImageRepository ProductImage { get; }
		IOrderSummaryRepository OrderSummary { get; }
		IOrderDetailRepository OrderDetail { get; }
        void Save();

		

	}
}

