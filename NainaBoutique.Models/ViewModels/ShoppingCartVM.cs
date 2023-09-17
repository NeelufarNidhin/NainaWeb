﻿using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models.ViewModels
{

	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart>? shoppingCartList { get; set; }
		public OrderSummary OrderSummary { get; set; }
	}
}

