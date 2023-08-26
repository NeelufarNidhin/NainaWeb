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
		[MaxLength(30)]
		public string? CategoryName { get; set; }
        [DisplayName("Sub Category")]
		[MaxLength(30)]
        public string? SubCategory { get; set; }
        [DisplayName("Description")]
		[MaxLength(100)]
        public string? Description { get; set; }
	}
}

