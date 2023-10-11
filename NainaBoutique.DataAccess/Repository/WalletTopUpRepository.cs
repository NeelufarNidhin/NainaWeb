using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository

    
{

  
    public class WalletTopUpRepository : Repository<WalletTopUp>, IWalletTopUpRepository
    {
        private readonly ApplicationDbContext _db;
        public WalletTopUpRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WalletTopUp walletTopup)
        {
            _db.WalletTopUps.Update(walletTopup);
        }
    }
}

