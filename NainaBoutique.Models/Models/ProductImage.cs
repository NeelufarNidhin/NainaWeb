using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NainaBoutique.Models.Models
{
	public class ProductImage
	{
		public int Id  { get; set; }
		public string ImageUrl  { get; set; }


		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		public ProductModel Product { get; set; }
		
	}
}

