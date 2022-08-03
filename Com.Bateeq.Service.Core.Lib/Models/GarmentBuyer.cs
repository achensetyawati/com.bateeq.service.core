using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentBuyer : StandardEntity, IValidatableObject
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
        public string City { get; set; }

        [StringLength(500)]
        public string Country { get; set; }

        [StringLength(500)]
        public string Contact { get; set; }

        public int? Tempo { get; set; }

        [StringLength(500)]
        public string Type { get; set; }

        [StringLength(100)]
        public string NPWP { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "Code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "Name" }));

            if (this.Tempo.Equals(null))
                this.Tempo = 0;
            else if (this.Tempo < 0)
                validationResult.Add(new ValidationResult("Tempo must be 0 or more", new List<string> { "Tempo" }));

            if (string.IsNullOrWhiteSpace(this.Country))
                validationResult.Add(new ValidationResult("Country is required", new List<string> { "Country" }));

            if (string.IsNullOrWhiteSpace(this.Type))
                validationResult.Add(new ValidationResult("Type is required", new List<string> { "Type" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                GarmentBuyerService service = (GarmentBuyerService)validationContext.GetService(typeof(GarmentBuyerService));

                if (service.DbContext.Set<GarmentBuyer>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "Code" }));
            }

            return validationResult;
        }
    }
}
