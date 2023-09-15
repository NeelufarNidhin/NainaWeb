using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IProductImageRepository : IRepository<ProductImage>
	{
		void Update(ProductImage productImage);
	}
}

