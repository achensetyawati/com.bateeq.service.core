using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class GarmentSection : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            GarmentSectionService service = validationContext.GetService<GarmentSectionService>();

            if (string.IsNullOrWhiteSpace(this.Code))
            {
                yield return new ValidationResult("Kode harus diisi", new List<string> { "Code" });
            }
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                yield return new ValidationResult("Nama harus diisi", new List<string> { "Name" });
            }
            //else
            //{
            //    if (service.DbSet.Count(r => r.Id != this.Id && r.Code.Equals(this.Code) && r.Name.Equals(this.Name) && r._IsDeleted.Equals(false)) > 0)
            //        yield return new ValidationResult("Nama komoditi sudah ada", new List<string> { "Name" });
            //}
            if (service.DbSet.Count(r => r.Id != this.Id && r.Code.Equals(this.Code) && r.Name.Equals(this.Name) && r._IsDeleted.Equals(false)) > 0)
            {
                yield return new ValidationResult("Seksi sudah ada", new List<string> { "Code" });
                yield return new ValidationResult("Seksi sudah ada", new List<string> { "Name" });
            }
        }
    }
}
