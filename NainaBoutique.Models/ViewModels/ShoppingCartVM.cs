using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models.ViewModels
{

	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart>? shoppingCartList { get; set; }
		public OrderSummary OrderSummary { get; set; }
        public IEnumerable<SelectListItem> CouponList { get; set; }
        public AppliedCoupon AppliedCoupon { get; set; }
		public  AddressModel AddressModel { get; set; }
    }
}

