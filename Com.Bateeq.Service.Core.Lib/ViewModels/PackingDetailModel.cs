using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class PackingDetailModel : IValidatableObject
    {
        [MaxLength(250)]
        public string Lot { get; set; }
        [MaxLength(100)]
        public string Grade { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public int Quantity { get; set; }
        [MaxLength(500)]
        public string Remark { get; set; }

        public int PackingId { get; set; }

        [ForeignKey("PackingId")]
        public virtual PackingModel Packing { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
