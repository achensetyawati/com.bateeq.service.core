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
    public class SizeModel : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        public string Size { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SizeService service = validationContext.GetService<SizeService>();
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Size))
            {
                validationResult.Add(new ValidationResult("Size is required", new List<string> { "Size" }));
            }
            else
            {
                if (service.DbSet.Count(r => r.Id != this.Id && r.Size.Equals(this.Size) && r._IsDeleted.Equals(false)) > 0)
                    validationResult.Add(new ValidationResult("Size already exists", new List<string> { "Size" }));
            }
            return validationResult;
        }
    }
}