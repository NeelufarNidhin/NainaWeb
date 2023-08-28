using System;
using NainaBoutique.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IProductRepository : IRepository<ProductModel>
	{
		void Update(ProductModel product);
	}
}

