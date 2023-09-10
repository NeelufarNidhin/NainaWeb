using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class FavouritesRepository : Repository<FavouritesModel>, IFavouritesRepository
    {

        private ApplicationDbContext _db;
        public FavouritesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(FavouritesModel favourite)
        {
            _db.Favourites.Update(favourite);
        }
    }
}

