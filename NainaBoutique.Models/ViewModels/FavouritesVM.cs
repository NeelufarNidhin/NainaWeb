using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models.ViewModels
{
	public class FavouritesVM
	{
        public IEnumerable<FavouritesModel>? favouritesList { get; set; }
        public IEnumerable<ShoppingCart>? shoppingCartList { get; set; }

    }
}

