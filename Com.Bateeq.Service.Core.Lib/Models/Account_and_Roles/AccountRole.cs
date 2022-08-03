//using Com.Moonlay.Models;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Text;

//namespace Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles
//{
//    public class AccountRole : StandardEntity, IValidatableObject
//    {
//        [MaxLength(255)]
//        public string UId { get; set; }
//        public int AccountId { get; set; }
//        //public virtual Account Account { get; set; }
//        public int RoleId { get; set; }
//        public virtual Role Role { get; set; }

//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            return new List<ValidationResult>();
//        }
//    }
//}
