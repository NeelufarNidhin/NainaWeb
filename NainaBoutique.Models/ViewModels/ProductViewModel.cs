using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NainaBoutique.Models.ViewModels
{
	public class ProductViewModel
	{
		public ProductModel Product { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem>? CategoryLlist { get; set; }
	}
}

