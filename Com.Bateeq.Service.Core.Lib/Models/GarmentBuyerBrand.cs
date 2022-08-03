using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentBuyerBrand : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "Code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "Name" }));

            if (BuyerCode == null || string.IsNullOrWhiteSpace(BuyerCode))
                validationResult.Add(new ValidationResult("Buyer Agent is required", new List<string> { "Buyers" }));
            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                GarmentBuyerBrandService service = (GarmentBuyerBrandService)validationContext.GetService(typeof(GarmentBuyerBrandService));

                if (service.DbContext.Set<GarmentBuyerBrand>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "Code" }));
                if (service.DbContext.Set<GarmentBuyerBrand>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Name already exists", new List<string> { "Name" }));
            }

            return validationResult;
        }
    }
}