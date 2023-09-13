using System;
using System.ComponentModel.DataAnnotations;

namespace NainaBoutique.Models.Models
{
	public class OtpModel
	{
        [Required]
        [DataType(DataType.Text)]
        public string? OtpCode { get; set; }
        public bool RememberMe { get; set; }
    }
}

