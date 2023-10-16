using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models.ViewModels
{
	public class ReportVM
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

        public OrderSummary? ordersummary { get; set; }
        public IEnumerable<OrderSummary> OrderSummary { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public IEnumerable<ProductModel> ProductDetail { get; set; }
        public ProductViewModel ProductVM { get; set; }
        public int Selected { get; set; }


        public List<OrderDetail> TrendingProductsData { get; set; }

        public List<OrderDetail> TotalRevenueData { get; set; }


        public List<OrderDetail> TotalOrdersData { get; set; }

    }
}

