using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository

    
{

   
    public class WalletRepository : Repository<WalletModel>, IWalletRepository
    {
        private readonly ApplicationDbContext _db;
        public WalletRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WalletModel wallet)
        {
            _db.WalletModels.Update(wallet);
        }
    }
}

