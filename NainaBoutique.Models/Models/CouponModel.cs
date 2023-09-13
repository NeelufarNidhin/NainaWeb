using System;
using System.ComponentModel.DataAnnotations;

namespace NainaBoutique.Models.Models
{
	public class CouponModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? CouponCode { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime Validfrom { get; set; }
        [Required]
        public DateTime ValidTo { get; set; }
        [Required]
        public string? DiscountType { get; set; }
        public float DiscountAmount { get; set; }
        [Required]
        public float MinCartAmount { get; set; }
        [Required]
        public float MaxRedeemableAmount { get; set; }


    }
}

