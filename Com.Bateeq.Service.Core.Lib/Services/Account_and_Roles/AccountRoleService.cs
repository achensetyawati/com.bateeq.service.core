//using Com.DanLiris.Service.Core.Lib.Helpers;
//using Com.DanLiris.Service.Core.Lib.Interfaces;
//using Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Com.DanLiris.Service.Core.Lib.Services.Account_and_Roles
//{
//    public class AccountRoleService : BasicService<CoreDbContext, AccountRole>
//    {
//        public AccountRoleService(IServiceProvider serviceProvider) : base(serviceProvider)
//        {

//        }

//        public override void OnCreating(AccountRole model)
//        {
//            base.OnCreating(model);
//            model._CreatedAgent = "Service";
//            model._CreatedBy = this.Username;
//        }
//        public override void OnUpdating(int id, AccountRole model)
//        {
//            base.OnUpdating(id, model);
//            model._LastModifiedAgent = "Service";
//            model._LastModifiedBy = this.Username;
//        }

//        public override void OnDeleting(AccountRole model)
//        {
//            base.OnDeleting(model);
//            model._LastModifiedAgent = "Service";
//            model._LastModifiedBy = this.Username;
//            model._DeletedAgent = "Service";
//            model._DeletedBy = this.Username;
//        }

//        public override Tuple<List<AccountRole>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
