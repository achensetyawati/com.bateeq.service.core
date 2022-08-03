using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Com.Moonlay.Models;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class MasterSource : StandardEntity, IValidatableObject
    {
        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
