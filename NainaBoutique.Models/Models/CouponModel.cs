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
       
        [Required]
        public DateTime ValidTill { get; set; }
        [Required]
        public float Discount { get; set; }
        [Required]
        public float MinCartAmount { get; set; }
        [Required]
        public float MaxAmount { get; set; }


    }
}

