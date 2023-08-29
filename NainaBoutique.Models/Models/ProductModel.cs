using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
		[Range(1,1000)]
        public float Price { get; set; }
        [Range(1, 1000)]
        public float  Sale_Price { get; set; }
       
        public int QuantityInStock { get; set; }
        [Required]
        public string? Color { get; set; }
		
		public int? CategoryId { get; set; }
		[ForeignKey("CategoryId")]
        [ValidateNever]
        public CategoryModel? Category { get; set; }
		[ValidateNever]
		public string? ImageUrl { get; set; }



	}
}

