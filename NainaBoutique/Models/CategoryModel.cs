using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NainaBoutique.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DisplayName("Category Name")]
		public string? CategoryName { get; set; }
        [DisplayName("Sub Category")]
        public string? SubCategory { get; set; }
        [DisplayName("Display Type")]
        public string? DisplayType { get; set; }
	}
}

