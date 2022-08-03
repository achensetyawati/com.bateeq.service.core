using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels.Account_and_Roles
{
    public class AccountViewModel : BasicViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public AccountProfileViewModel Profile { get; set; }
        public List<RoleViewModel> Roles { get; set; }

    }
}
