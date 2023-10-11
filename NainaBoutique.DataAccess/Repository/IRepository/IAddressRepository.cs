using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IAddressRepository : IRepository<AddressModel>
	{
		void Update(AddressModel addressModel);
	}
}

