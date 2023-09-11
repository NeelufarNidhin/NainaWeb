using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
	public class GiftcardRepository : Repository<GiftcardModel> , IGiftcardRepository
	{
        private ApplicationDbContext _db;
		public GiftcardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
		}

        public void Update(GiftcardModel giftcard)
        {
            _db.Giftcards.Update(giftcard);
        }
    }
}

