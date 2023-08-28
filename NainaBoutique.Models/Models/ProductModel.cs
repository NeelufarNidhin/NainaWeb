using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NainaBoutique.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? ProductName { get; set; }
		[Required]
        public string? Description { get; set; }
		[Required]
        public float Price { get; set; }
		public float  Sale_Price { get; set; }
		public int QuantityInStock { get; set; }
        public string? Color { get; set; }
		
		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
        public CategoryModel? Category { get; set; }
		public string? ImageUrl { get; set; }



	}
}

