using System;
using System.Linq.Expressions;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T:class
	{
		//T-CategoryModel

		IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter=null, string? includeProperties = null);
		//Linq operation in Controller
		T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = false);
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);
	}

}

