using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class AccountBank : StandardEntity, IValidatableObject
    {
        [MaxLength(128)]
        public string UId { get; set; }
        [StringLength(64)]
        public string Code { get; set; }
        [StringLength(64)]
        public string BankCode { get; set; }
        [StringLength(512)]
        public string BankName { get; set; }
        [StringLength(512)]
        public string BankAddress { get; set; }
        [StringLength(512)]
        public string AccountName { get; set; }
        [StringLength(64)]
        public string AccountCOA { get; set; }
        [StringLength(128)]
        public string AccountNumber { get; set; }
        [StringLength(256)]
        public string SwiftCode { get; set; }
        public int? CurrencyId { get; set; }
        [StringLength(32)]
        public string CurrencyCode { get; set; }
        [StringLength(8)]
        public string CurrencySymbol { get; set; }
        /* Double */
        public double CurrencyRate { get; set; }
        [StringLength(1024)]
        public string CurrencyDescription { get; set; }
        [StringLength(64)]
        //Division
        public string DivisionName { get; set; }
        [StringLength(32)]
        public string DivisionCode { get; set; }
        public int? DivisionId { get; set; }
        [StringLength(128)]
        public string Phone { get; set; }
        [StringLength(128)]
        public string Fax { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.BankName))
                validationResult.Add(new ValidationResult("Bank Name is required", new List<string> { "BankName" }));

            if (string.IsNullOrWhiteSpace(this.AccountName))
                validationResult.Add(new ValidationResult("Account Name is required", new List<string> { "AccountName" }));

            if (string.IsNullOrWhiteSpace(this.AccountNumber))
                validationResult.Add(new ValidationResult("Account Number is required", new List<string> { "AccountNumber" }));

            if (CurrencyId.Equals(null))
                validationResult.Add(new ValidationResult("Currency is required", new List<string> { "CurrencyID" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                AccountBankService service = (AccountBankService)validationContext.GetService(typeof(AccountBankService));

                if (service.DbContext.Set<AccountBank>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.BankName.Equals(this.BankName) && r.AccountNumber.Equals(this.AccountNumber)) > 0) /* Unique */
                {
                    validationResult.Add(new ValidationResult("Bank Name and Account Number already exists", new List<string> { "BankName" }));
                    validationResult.Add(new ValidationResult("Bank Name and Account Number already exists", new List<string> { "AccountNumber" }));
                }
            }

            return validationResult;
        }
    }
}
