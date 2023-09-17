using System;
using System.ComponentModel.DataAnnotations;

namespace NainaBoutique.Models.Models
{
	public class OtpModel
	{
        [Key]
        public int Id { get; set; }
        public string? Otp { get; set; }
        public string? Email { get; set; }
    }
}

