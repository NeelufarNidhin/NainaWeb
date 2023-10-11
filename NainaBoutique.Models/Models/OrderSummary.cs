using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NainaBoutique.Models.Models
{
	public class OrderSummary
	{
		[Key]
		public int Id{ get; set; }
		public string? ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser? ApplicationUser { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime  ShippingDate { get; set; }
		public float OrderTotal { get; set; }
		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
        [Required]
        public string? PaymentMethod { get; set; }
        public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }
		public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string? PaymentIntendId { get; set; }
		public string? SessionId { get; set; }

	

        [Required]
		public string? Name { get; set; }
		[Required]
		public int MobileNumber { get; set; }
		[Required]
		public string? Address { get; set; }
		[Required]
		public string? City { get; set; }
		[Required]
		public string? State { get; set; }
		public int PostalCode { get; set; }

}
}

