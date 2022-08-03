using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels.Account_and_Roles
{
    public class PermissionViewModel : BasicViewModel
    {
        public UnitViewModel unit { get; set; }
        public int permission { get; set; }
        public int RoleId { get; set; }
    }
}
