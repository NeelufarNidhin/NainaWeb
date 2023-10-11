using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Models
{
	public class ProductModel : ISoftDelete
	{
		[Key]
		public int Id { get; set; }


        [MaxLength(30)]
        [Required]
		public string? ProductName { get; set; }

        [Required]
        public string? Description { get; set; }
		[Required]
		[Range(1,1000)]
        public float Price { get; set; }
        [Range(1, 1000)]
        public float  Sale_Price { get; set; }
        [Range(0, 1000)]
        public int QuantityInStock { get; set; }
        [Required]
        [MaxLength(20)]
        public string? Color { get; set; }
        [MaxLength(10)]
        [Required]
		public string? Size { get; set; }
		
		public int? CategoryId { get; set; }
		[ForeignKey("CategoryId")]
        [ValidateNever]
        public CategoryModel? Category { get; set; }

		[ValidateNever]
		public List<ProductImage>? ProductImage { get; set; }

        public char RecStatus { get; set; } = 'A';
        public DateTime CreatedAt { get; set; } = DateTime.Now;




    }
}

