using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NainaBoutique.Models.Models
{
	public class ShoppingCart
	{
		[Key]
		public int  Id{ get; set; }
        
		public int  ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public ProductModel? Product { get; set; }
       //[NotMapped ]
		public decimal Price { get; set; }

        [Range(1, 1000, ErrorMessage = "Enter the value between 1 and 1000")]
        public int Count { get; set; }


        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

       





    }
}

