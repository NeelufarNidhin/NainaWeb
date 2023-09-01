﻿using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace NainaBoutique.Models
{

public class ApplicationUser : IdentityUser
{
        [Required]
		public string? Name  { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
        public string? State{ get; set; }
        public int PostalCode { get; set; }
        public int MobileNumber { get; set; }

    }

}