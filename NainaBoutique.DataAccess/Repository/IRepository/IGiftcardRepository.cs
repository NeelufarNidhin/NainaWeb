using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IGiftcardRepository : IRepository<GiftcardModel>
	{
		void Update(GiftcardModel giftcard);
	}
}

