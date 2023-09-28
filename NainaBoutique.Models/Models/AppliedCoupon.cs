using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NainaBoutique.Models.Models
{
	public class AppliedCoupon
	{
		[Key]
		public int Id  { get; set; }

		public string? UserId { get; set; }
		[ForeignKey("UserId")]
		[ValidateNever]
		public ApplicationUser? ApplicationUser { get; set; }

		public int CouponId { get; set; }
		[ForeignKey("CouponId")]
		[ValidateNever]
		public CouponModel? Coupon { get; set; }

		public bool AppliedStatus { get; set; }
    }
}

