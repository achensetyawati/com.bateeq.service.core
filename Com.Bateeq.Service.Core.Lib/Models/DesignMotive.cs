using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class DesignMotive : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        [StringLength(25)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            return validationResult;
        }
    }
}
