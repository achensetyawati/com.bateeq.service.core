using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels.Account_and_Roles
{
    public class RoleViewModel : BasicViewModel, IValidatableObject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<PermissionViewModel> Permissions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int Count = 0;

            if (string.IsNullOrWhiteSpace(this.Code))
                yield return new ValidationResult("Code is required", new List<string> { "code" });

            if (string.IsNullOrWhiteSpace(this.Name))
                yield return new ValidationResult("Name is required", new List<string> { "name" });

            string permissionError = "[";

            foreach (PermissionViewModel permission in Permissions)
            {
                if (string.IsNullOrWhiteSpace(permission.unit.Name))
                {
                    Count++;
                    permissionError += "{ unit: 'Unit is required' }, ";
                }
                else
                {
                    permissionError += "{}, ";
                }
            }

            permissionError += "]";

            if (Count > 0)
            {
                yield return new ValidationResult(permissionError, new List<string> { "unit" });
            }
        }
    }
}
