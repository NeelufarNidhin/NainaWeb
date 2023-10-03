using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models.ViewModels
{
	public class OrderVM
	{
		public OrderSummary OrderSummary { get; set; }
		public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public IEnumerable<ProductModel> ProductDetail { get; set; }
        public IEnumerable<ProductImage> ImageDetail { get; set; }
    }
}

