using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NainaBoutique.Models.Models
{
	public class AddressModel
	{
        
            [Key]
            public int Id { get; set; }

            public string? UserId { get; set; }
            [ForeignKey("UserId")]
            [ValidateNever]

            public ApplicationUser? ApplicationUser { get; set; }
            public string? Address { get; set; }
            public string? State { get; set; }
            public string? City { get; set; }
            public int PostalCode { get; set; }
            public int MobileNumber { get; set; }


    }
}

