using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Supplier : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(3000)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Contact { get; set; }

        [StringLength(500)]
        public string PIC { get; set; }
        
        public bool? Import { get; set; }

        [StringLength(100)]
        public string NPWP { get; set; }

        [StringLength(500)]
        public string SerialNumber { get; set; }        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if (this.Import.Equals(null))
                this.Import = false;

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                SupplierService service = (SupplierService)validationContext.GetService(typeof(SupplierService));

                if (service.DbContext.Set<Supplier>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "code" }));
            }
            
            return validationResult;
        }
    }
}
