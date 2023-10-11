using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IWalletTopUpRepository : IRepository<WalletTopUp>
	{
		void Update( WalletTopUp walletTopup);
	}
}

