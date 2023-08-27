using System;
using NainaBoutique.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface ICategoryRepository : IRepository<CategoryModel>
	{
		void Update(CategoryModel category);
		
	}
}

