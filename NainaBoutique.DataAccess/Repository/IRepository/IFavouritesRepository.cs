using System;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IFavouritesRepository: IRepository<FavouritesModel>
	{
        void Update(FavouritesModel favourite);
    }
}

