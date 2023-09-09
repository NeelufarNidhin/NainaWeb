using System;
using System.ComponentModel.DataAnnotations;

namespace NainaBoutique.Models.Models
{
	public class SizeModel
	{
		[Key]
		public int  ID{ get; set; }
		public string? Size { get; set; }
	}
}

