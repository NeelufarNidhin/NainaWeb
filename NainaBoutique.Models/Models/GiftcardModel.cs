using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace NainaBoutique.Models.Models
{
	public class GiftcardModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public float Amount { get; set; }
        public string? EmailReceipient { get; set; }
		public string? Message { get; set; }
        [Required]
        public string? CardNumber { get; set; }
		public string? IsActive { get; set; }
		public DateTime ExpiryDate { get; set; }
        public int OrderId { get; set; }
		
        public int UserrId { get; set; }

    }
}

