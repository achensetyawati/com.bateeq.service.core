using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Com.DanLiris.Service.Core.Lib.Services;
using System.Linq;
using Com.DanLiris.Service.Core.Lib.Helpers;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentCurrency : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

		public DateTime Date { get; set; }

        public double? Rate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "Code" }));

            if (this.Date > DateTime.Now)
                validationResult.Add(new ValidationResult("Date must be less than or equal today's date", new List<string> { "Date" }));

            if (this.Rate.Equals(null) || this.Rate < 0)
                validationResult.Add(new ValidationResult("Rate must be greater than zero", new List<string> { "Rate" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                GarmentCurrencyService service = (GarmentCurrencyService)validationContext.GetService(typeof(GarmentCurrencyService));
				var coba = service.DbContext.Set<GarmentCurrency>().Where(r => r.Code == this.Code);
                if (service.DbContext.Set<GarmentCurrency>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code) && r.Date.Equals(this.Date)) > 0) /* Unique */
                {
                    validationResult.Add(new ValidationResult("Code and Date already exists", new List<string> { "Code" }));
                    validationResult.Add(new ValidationResult("Code and Date already exists", new List<string> { "Date" }));
                }
            }

            return validationResult;
        }
    }
}
