using System;
using System.ComponentModel.DataAnnotations;

namespace NainaBoutique.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? CategoryName { get; set; }
		public string? SubCategory { get; set; }
		public string? DisplayType { get; set; }
	}
}

