using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class IncomeTax : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public double? Rate { get; set; }
        
        public string Description { get; set; }        

        public string COACodeCredit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if (this.Rate.Equals(null) || this.Rate < 0)
                validationResult.Add(new ValidationResult("Rate must be greater than zero", new List<string> { "rate" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                IncomeTaxService service = (IncomeTaxService)validationContext.GetService(typeof(IncomeTaxService));

                if (service.DbContext.Set<IncomeTax>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name) && r.Rate.Equals(this.Rate)) > 0) /* Name and Rate Unique */
                {
                    validationResult.Add(new ValidationResult("Name and Rate already exists", new List<string> { "name" }));
                    validationResult.Add(new ValidationResult("Name and Rate already exists", new List<string> { "rate" }));
                }
            }
            return validationResult;
        }
    }
}
