using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface ICartRepository : IRepository<ShoppingCart>
	{
		void Update(ShoppingCart cart);
	}
}

