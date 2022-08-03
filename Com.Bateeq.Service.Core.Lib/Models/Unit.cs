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
    public class Unit : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        public int DivisionId { get; set; }

        [StringLength(100)]
        public string DivisionCode { get; set; }

        [StringLength(500)]
        public string DivisionName { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public string Description { get; set; }    
        [MaxLength(50)]
        public string COACode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if(validationResult.Count.Equals(0))
            {
                UnitService service = (UnitService)validationContext.GetService(typeof(UnitService));

                if (service.DbContext.Set<Unit>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "code" }));

                if (service.DbContext.Set<Unit>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Name already exists", new List<string> { "name" }));
            }

            if (!string.IsNullOrWhiteSpace(COACode) && COACode.Count() != 1)
                validationResult.Add(new ValidationResult("Kode COA tidak valid.", new List<string> { "COACode" }));

            return validationResult;
        }
    }
}
