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
        [Range(1, 1000)]
        public decimal Discount { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal MinCartAmount { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal MaxAmount { get; set; }


    }
}

