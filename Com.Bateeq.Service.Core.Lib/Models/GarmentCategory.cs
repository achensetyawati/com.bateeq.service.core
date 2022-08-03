using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentCategory : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string CodeRequirement { get; set; }
        public int? UomId { get; set; }
        [MaxLength(255)]
        public string UomUnit { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "Code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "Name" }));

            if (string.IsNullOrWhiteSpace(this.CodeRequirement))
                validationResult.Add(new ValidationResult("CodeRequirement is required", new List<string> { "CodeRequirement" }));

            if (this.UomId.Equals(null))
                validationResult.Add(new ValidationResult("Uom is required", new List<string> { "UomId" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                GarmentCategoryService service = (GarmentCategoryService)validationContext.GetService(typeof(GarmentCategoryService));

                if (service.DbContext.Set<GarmentCategory>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name) ) > 0) /* Unique */
                {
                    validationResult.Add(new ValidationResult("Name already exists", new List<string> { "Name" }));
                }
            }

            return validationResult;
        }
    }
}
