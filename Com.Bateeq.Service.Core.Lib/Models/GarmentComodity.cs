using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentComodity : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            GarmentComodityService service = validationContext.GetService<GarmentComodityService>();

            if (string.IsNullOrWhiteSpace(this.Code))
            {
                yield return new ValidationResult("Kode harus diisi", new List<string> { "Code" });
            }

            else if (service.DbSet.Count(r => r.Id != this.Id && r.Code.Equals(this.Code) && r._IsDeleted.Equals(false)) > 0)
            {
                yield return new ValidationResult("Kode Komoditi sudah ada", new List<string> { "Code" });
            }

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                yield return new ValidationResult("Nama harus diisi", new List<string> { "Name" });
            }
        }
    }
}
