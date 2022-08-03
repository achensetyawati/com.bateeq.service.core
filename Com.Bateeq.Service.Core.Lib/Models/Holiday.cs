using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Holiday : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }
        
        public DateTime? Date { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int? DivisionId { get; set; }

        [StringLength(500)]
        public string DivisionName { get; set; }

        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Date.Equals(null))
                yield return new ValidationResult("Date is required", new List<string> { "date" });

            if (string.IsNullOrWhiteSpace(this.Name))
                yield return new ValidationResult("Name is required", new List<string> { "name" });            

            if(this.DivisionId.Equals(null))
                yield return new ValidationResult("Division is required", new List<string> { "division" });

            if (string.IsNullOrWhiteSpace(this.Description))
                yield return new ValidationResult("Description is required", new List<string> { "description" });
        }
    }
}
