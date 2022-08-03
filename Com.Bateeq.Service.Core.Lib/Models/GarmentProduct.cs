using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentProduct : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        [StringLength(100)]
        public string Code { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public int? UomId { get; set; }
        [StringLength(500)]
        public string UomUnit { get; set; }
        public string ProductType { get; set; }
        public string Composition { get; set; }
        [StringLength(500)]
        public string Const { get; set; }
        [StringLength(500)]
        public string Yarn { get; set; }
        public string Width { get; set; }
        [StringLength(500)]
        public string Tags { get; set; }
        public string Remark { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if (string.IsNullOrWhiteSpace(this.UomUnit))
                validationResult.Add(new ValidationResult("Uom is required", new List<string> { "uom" }));

            if (string.IsNullOrWhiteSpace(this.Composition) && this.ProductType == "FABRIC")
                validationResult.Add(new ValidationResult("Composition is required", new List<string> { "composition" }));

            if (string.IsNullOrWhiteSpace(this.Const) && this.ProductType == "FABRIC")
                validationResult.Add(new ValidationResult("Const is required", new List<string> { "const" }));

            if (string.IsNullOrWhiteSpace(this.Yarn) && this.ProductType == "FABRIC")
                validationResult.Add(new ValidationResult("Yarn is required", new List<string> { "yarn" }));

            if (string.IsNullOrWhiteSpace(this.Width) && this.ProductType == "FABRIC")
                validationResult.Add(new ValidationResult("Width is required", new List<string> { "width" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                GarmentProductService service = (GarmentProductService)validationContext.GetService(typeof(GarmentProductService));

                if (service.DbContext.Set<GarmentProduct>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0)
                    validationResult.Add(new ValidationResult("Code is already exist", new List<string> { "code" }));

                if (service.DbContext.Set<GarmentProduct>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name) && this.ProductType.Equals("NON FABRIC")) > 0) /* Name Unique */
                    validationResult.Add(new ValidationResult("Name already exists", new List<string> { "name" }));

                if (service.DbContext.Set<GarmentProduct>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && this.ProductType.Equals("FABRIC") && r.Composition.Equals(this.Composition) && r.Const.Equals(this.Const) && r.Yarn.Equals(this.Yarn) && r.Width.Equals(this.Width)) >0)
                    validationResult.Add(new ValidationResult("Product with same Composition, Const, Yarn, Width already exists", new List<string> { "combinationerror" }));
            }

            return validationResult;
        }
    }
}
