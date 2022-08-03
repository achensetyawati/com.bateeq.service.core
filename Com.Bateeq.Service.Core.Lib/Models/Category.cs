using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Category : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string CodeRequirement { get; set; }

        public int DivisionId { get; set; }

        [MaxLength(50)]
        public string PurchasingCOA { get; set; }
        [MaxLength(50)]
        public string StockCOA { get; set; }
        [MaxLength(50)]
        public string LocalDebtCOA { get; set; }
        [MaxLength(50)]
        public string ImportDebtCOA { get; set; }

        //public Division Division { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                CategoryService service = (CategoryService)validationContext.GetService(typeof(CategoryService));

                if (service.DbContext.Set<Category>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "code" }));

                if (service.DbContext.Set<Category>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name)) > 0) /* Name Unique */
                    validationResult.Add(new ValidationResult("Name already exists", new List<string> { "name" }));
            }

            if (!string.IsNullOrWhiteSpace(PurchasingCOA))
            {
                var splittedString = PurchasingCOA.Split('.');
                if (splittedString.Count() != 2)
                    validationResult.Add(new ValidationResult("Kode COA harus dipisah menggunakan titik.", new List<string> { "PurchasingCOA" }));
                else
                {
                    var firstCodeCount = splittedString[0].Count();
                    var secondCodeCount = splittedString[1].Count();

                    if (firstCodeCount != 4 || secondCodeCount != 2)
                        validationResult.Add(new ValidationResult("Kode COA tidak valid.", new List<string> { "PurchasingCOA" }));
                }
            }

            if (!string.IsNullOrWhiteSpace(StockCOA))
            {
                var splittedString = StockCOA.Split('.');
                if (splittedString.Count() != 2)
                    validationResult.Add(new ValidationResult("Kode COA harus dipisah menggunakan titik.", new List<string> { "StockCOA" }));
                else
                {
                    var firstCodeCount = splittedString[0].Count();
                    var secondCodeCount = splittedString[1].Count();

                    if (firstCodeCount != 4 || secondCodeCount != 2)
                        validationResult.Add(new ValidationResult("Kode COA tidak valid.", new List<string> { "StockCOA" }));
                }
            }

            if (!string.IsNullOrWhiteSpace(LocalDebtCOA))
            {
                var splittedString = LocalDebtCOA.Split('.');
                if (splittedString.Count() != 2)
                    validationResult.Add(new ValidationResult("Kode COA harus dipisah menggunakan titik.", new List<string> { "LocalDebtCOA" }));
                else
                {
                    var firstCodeCount = splittedString[0].Count();
                    var secondCodeCount = splittedString[1].Count();

                    if (firstCodeCount != 4 || secondCodeCount != 2)
                        validationResult.Add(new ValidationResult("Kode COA tidak valid.", new List<string> { "LocalDebtCOA" }));
                }
            }

            if (!string.IsNullOrWhiteSpace(ImportDebtCOA))
            {
                var splittedString = ImportDebtCOA.Split('.');
                if (splittedString.Count() != 2)
                    validationResult.Add(new ValidationResult("Kode COA harus dipisah menggunakan titik.", new List<string> { "ImportDebtCOA" }));
                else
                {
                    var firstCodeCount = splittedString[0].Count();
                    var secondCodeCount = splittedString[1].Count();

                    if (firstCodeCount != 4 || secondCodeCount != 2)
                        validationResult.Add(new ValidationResult("Kode COA tidak valid.", new List<string> { "ImportDebtCOA" }));
                }
            }

            return validationResult;
        }
    }
}
