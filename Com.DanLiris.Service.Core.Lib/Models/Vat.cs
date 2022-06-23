using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
	public class Vat : StandardEntity, IValidatableObject
	{
		[StringLength(500)]
		public string Name { get; set; }

		public double? Rate { get; set; }

		public string Description { get; set; }
		

		public DateTimeOffset? Date { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> validationResult = new List<ValidationResult>();

			if (string.IsNullOrWhiteSpace(this.Name))
				validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

			if (this.Rate.Equals(null) || this.Rate < 0)
				validationResult.Add(new ValidationResult("Rate must be greater than zero", new List<string> { "rate" }));
 
			return validationResult;
		}
	}
}
