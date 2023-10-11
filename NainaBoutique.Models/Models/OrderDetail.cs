using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NainaBoutique.Models.Models
{
	public class OrderDetail
	{
		[Key]
		public int  Id { get; set; }
		public int OrderSummaryId { get; set; }
		[ForeignKey("OrderSummaryId")]
		[ValidateNever]
		public OrderSummary? OrderSummary { get; set; }

		[Required]
		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public ProductModel? Product { get; set; }
		public int Count { get; set; }
		public string? Size { get; set; }
        public float Price { get; set; }

    }
}

