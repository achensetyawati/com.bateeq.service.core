using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class StandardMinuteValue : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }
        public int BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public int ComodityId { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityName { get; set; }
        public DateTimeOffset SMVDate { get; set; }
        public decimal SMVCutting { get; set; }
        public decimal SMVSewing { get; set; }
        public decimal SMVFinishing { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(this.BuyerName))
            {
                yield return new ValidationResult("Buyer Harus Diisi", new List<string> { "BuyerName" });
            }
            if (string.IsNullOrWhiteSpace(this.ComodityName))
            {
                yield return new ValidationResult("Komoditi harus diisi", new List<string> { "ComodityName" });
            }
            if (this.SMVCutting <= 0)
            {
                yield return new ValidationResult("Cutting Harus Lebih dari 0", new List<string> { "SMVCutting" });
            }
            if (this.SMVSewing <= 0)
            {
                yield return new ValidationResult("Sewing Harus Lebih dari 0", new List<string> { "SMVSewing" });
            }
            if (this.SMVFinishing <= 0)
            {
                yield return new ValidationResult("Finishing Harus Lebih dari 0", new List<string> { "SMVFinishing" });
            }
        }
    }
}
