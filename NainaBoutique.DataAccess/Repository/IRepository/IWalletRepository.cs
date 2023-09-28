using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IWalletRepository : IRepository<WalletModel>
	{
		void Update( WalletModel wallet);
	}
}

