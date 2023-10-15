using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models.ViewModels
{
	public class AddressVM
	{
        public IEnumerable< AddressModel >Address { get; set; }
        public IEnumerable<ShoppingCart>? shoppingCartList { get; set; }
        public OrderSummary OrderSummary { get; set; }
    }
}

