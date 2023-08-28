using System;
using System.ComponentModel.DataAnnotations;

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
		

    }
}

