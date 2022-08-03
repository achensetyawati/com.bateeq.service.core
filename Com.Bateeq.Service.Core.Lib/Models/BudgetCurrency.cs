using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class BudgetCurrency : StandardEntity, IValidatableObject
    {
		[MaxLength(255)]
		public string UId { get; set; }

		[StringLength(100)]
		public string Code { get; set; }

		public DateTime Date { get; set; }

		public double? Rate { get; set; }

		public string Remark { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> validationResult = new List<ValidationResult>();

			if (string.IsNullOrWhiteSpace(this.Code))
				validationResult.Add(new ValidationResult("Code is required", new List<string> { "Code" }));

			if (this.Date > DateTime.Now)
				validationResult.Add(new ValidationResult("Date must be less than or equal today's date", new List<string> { "Date" }));

			if (this.Rate.Equals(null) || this.Rate < 0 || this.Rate == 0)
				validationResult.Add(new ValidationResult("Rate must be greater than zero", new List<string> { "Rate" }));

			if (validationResult.Count.Equals(0))
			{
				/* Service Validation */
				BudgetCurrencyService service = (BudgetCurrencyService)validationContext.GetService(typeof(BudgetCurrencyService));
				if (service.DbContext.Set<BudgetCurrency>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code) && r.Date.Equals(this.Date)) > 0) /* Unique */
				{
					validationResult.Add(new ValidationResult("Code and Date already exists", new List<string> { "Code" }));
					validationResult.Add(new ValidationResult("Code and Date already exists", new List<string> { "Date" }));
				}
			}

			return validationResult;
		}
	}
}
