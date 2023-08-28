using System;
namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        void Save();

		

	}
}

