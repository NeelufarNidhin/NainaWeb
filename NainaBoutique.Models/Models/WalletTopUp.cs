using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NainaBoutique.Models.Models
{
	public class WalletTopUp
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public float WalletBalance { get; set; }

        public DateTime PurchaseDate { get; set; }


        public string? Userid { get; set; }
        [ForeignKey("Userid")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

       
    }
}

