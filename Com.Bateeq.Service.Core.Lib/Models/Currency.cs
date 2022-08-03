using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Currency : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Symbol { get; set; }

        public double? Rate { get; set; }

        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "Code" }));
                
            if (string.IsNullOrWhiteSpace(this.Symbol))
                validationResult.Add(new ValidationResult("Symbol is required", new List<string> { "Symbol" }));

            if (this.Rate.Equals(null) || this.Rate < 0)
                validationResult.Add(new ValidationResult("Rate must be greater than zero", new List<string> { "Rate" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                CurrencyService service = (CurrencyService)validationContext.GetService(typeof(CurrencyService));

                if (service.DbContext.Set<Currency>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "Code" }));

                if (service.DbContext.Set<Currency>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Description.Equals(this.Description)) > 0) /* Description Unique */
                    validationResult.Add(new ValidationResult("Description already exists", new List<string> { "Description" }));
            }

            return validationResult;
        }
    }
}
