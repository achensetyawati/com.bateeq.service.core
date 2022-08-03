using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class MasterStore : StandardEntity, IValidatableObject
    {
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(255)]
        public string City { get; set; }
        public DateTimeOffset ClosedDate { get; set; }
        [StringLength(255)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public float MonthlyTotalCost { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string OnlineOffline { get; set; }
        public DateTimeOffset OpenedDate { get; set; }
        [StringLength(255)]
        public string Pic { get; set; }
        [StringLength(255)]
        public string Phone { get; set; }
        public float SalesCapital { get; set; }
        [StringLength(255)]
        public string SalesCategory { get; set; }
        public float SalesTarget { get; set; }
        [StringLength(255)]
        public string Status { get; set; }
        [StringLength(255)]
        public string StoreArea { get; set; }
        [StringLength(255)]
        public string StoreCategory { get; set; }
        [StringLength(255)]
        public string StoreWide { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }


        [StringLength(255)]
        public string Uid { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(this.Code))
                yield return new ValidationResult("Kode tidak boleh kosong", new List<string> { "code" });

            if (string.IsNullOrWhiteSpace(this.Name))
                yield return new ValidationResult("Nama tidak boleh kosong", new List<string> { "name" });
        }

    }
}
