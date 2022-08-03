using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class OrderType : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        [MaxLength(25)]
        public string Code { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Remark { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ComodityService service = validationContext.GetService<ComodityService>();

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                yield return new ValidationResult("Nama harus diisi", new List<string> { "Name" });
            }
            else
            {
                if (service.DbSet.Count(r => r.Id != this.Id && r.Name.Equals(this.Name) && r._IsDeleted.Equals(false)) > 0)
                    yield return new ValidationResult("Nama Jenis Order sudah ada", new List<string> { "Name" });
            }

        }
    }
}
