using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Uom : StandardEntity, IValidatableObject
    {
        
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(500)]
        public string Unit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Unit))
                validationResult.Add(new ValidationResult("Unit is required", new List<string> { "Unit" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                UomService service = (UomService)validationContext.GetService(typeof(UomService));

                if (service.DbContext.Set<Uom>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Unit.Equals(this.Unit)) > 0) /* Unit Unique */
                    validationResult.Add(new ValidationResult("Unit already exists", new List<string> { "Unit" }));
            }

            return validationResult;
        }
    }
}
