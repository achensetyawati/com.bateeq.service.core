//using Com.Moonlay.Models;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Text;

//namespace Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles
//{
//    public class Permission : StandardEntity, IValidatableObject
//    {
//        [MaxLength(255)]
//        public string UId { get; set; }
//        public int UnitId { get; set; }
//        public string UnitCode { get; set; }
//        public string Unit { get; set; }
//        public string Division { get; set; }
//        public int permission { get; set; }

//        public int RoleId { get; set; }
//        public virtual Role Role { get; set; }

//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
