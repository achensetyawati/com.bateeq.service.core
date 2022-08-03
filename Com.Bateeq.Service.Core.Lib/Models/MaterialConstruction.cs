using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Linq;
using Com.Moonlay.Models;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class MaterialConstruction : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            YarnMaterialService service = validationContext.GetService<YarnMaterialService>();
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                yield return new ValidationResult("Nama harus diisi", new List<string> { "Name" });
            }
            else
            {
                if (service.DbSet.Count(r => r.Id != this.Id && r.Name.Equals(this.Name) && r._IsDeleted.Equals(false)) > 0)
                    yield return new ValidationResult("Nama Material sudah ada", new List<string> { "Name" });
            }

        }
    }
}
