//using Com.DanLiris.Service.Core.Lib.Helpers;
//using Com.DanLiris.Service.Core.Lib.Interfaces;
//using Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles;
//using Com.DanLiris.Service.Core.Lib.ViewModels.Account_and_Roles;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Com.DanLiris.Service.Core.Lib.Services.Account_and_Roles
//{
//    public class AccountService : BasicService<CoreDbContext, Account>, IMap<Account, AccountViewModel>
//    {
//        private AccountRoleService AccountRoleService;

//        public AccountService(IServiceProvider serviceProvider, AccountRoleService accountRoleService) : base(serviceProvider)
//        {
//            this.AccountRoleService = accountRoleService;
//        }
//        public Account MapToModel(AccountViewModel accountVM)
//        {
//            Account account = new Account();
//            account.AccountProfile = new AccountProfile();
//            account.AccountRoles = new List<AccountRole>();

//            account.Username = accountVM.Username;
//            account.Password = accountVM.Password;
//            account.IsLocked = accountVM.IsLocked;
//            account.AccountProfile.Id = accountVM.Profile.Id;
//            account.AccountProfile.Firstname = accountVM.Profile.Firstname;
//            account.AccountProfile.Lastname = accountVM.Profile.Lastname;
//            account.AccountProfile.Gender = accountVM.Profile.Gender;
//            account.AccountProfile.AccountId = accountVM.Id;

//            foreach (RoleViewModel roleVM in accountVM.Roles)
//            {
//                AccountRole accountRole = new AccountRole();
//                accountRole.RoleId = roleVM.Id;

//                account.AccountRoles.Add(accountRole);
//            }

//            return account;
//        }

//        public AccountViewModel MapToViewModel(Account account)
//        {
//            AccountViewModel accountVM = new AccountViewModel();
//            accountVM.Profile = new AccountProfileViewModel();
//            accountVM.Roles = new List<RoleViewModel>();

//            accountVM.Username = account.Username;
//            accountVM.Password = account.Password;
//            accountVM.IsLocked = account.IsLocked;
//            accountVM.Profile.Id = account.AccountProfile.Id;
//            accountVM.Profile.Firstname = account.AccountProfile.Firstname;
//            accountVM.Profile.Lastname = account.AccountProfile.Lastname;
//            accountVM.Profile.Gender = account.AccountProfile.Gender;

//            if (account.AccountRoles != null)
//            {
//                foreach (AccountRole accountRole in account.AccountRoles)
//                {
//                    RoleViewModel roleViewModel = new RoleViewModel();
//                    roleViewModel.Id = accountRole.RoleId;
//                    roleViewModel.Name = accountRole.Role.Name;
//                    roleViewModel.Code = accountRole.Role.Code;

//                    accountVM.Roles.Add(roleViewModel);
//                }
//            }
//            return accountVM;
//        }

//        public override Tuple<List<Account>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
//        {
//            IQueryable<Account> Query = this.DbContext.Account;
//            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
//            Query = ConfigureFilter(Query, FilterDictionary);
//            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

//            /* Search With Keyword */
//            if (Keyword != null)
//            {
//                List<string> SearchAttributes = new List<string>()
//                {
//                    "Name","Code"
//                };

//                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
//            }

//            /* Const Select */
//            List<string> SelectedFields = new List<string>()
//            {
//                "Id", "Code", "Name", "Description","Permissions", "_LastModifiedUtc"
//            };

//            Query = Query
//                .Select(b => new Role
//                {
//                    Id = b.Id,
//                    Code = b.Code,
//                    Name = b.Name,
//                    Description = b.Description,
//                    Permissions = b.Permissions,
//                    _LastModifiedUtc = b._LastModifiedUtc
//                });

//            /* Order */
//            if (OrderDictionary.Count.Equals(0))
//            {
//                OrderDictionary.Add("_LastModifiedUtc", General.DESCENDING);

//                Query = Query.OrderByDescending(b => b._LastModifiedUtc); /* Default Order */
//            }
//            else
//            {
//                string Key = OrderDictionary.Keys.First();
//                string OrderType = OrderDictionary[Key];
//                string TransformKey = General.TransformOrderBy(Key);

//                BindingFlags IgnoreCase = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

//                Query = OrderType.Equals(General.ASCENDING) ?
//                    Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
//                    Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
//            }

//            /* Pagination */
//            Pageable<Role> pageable = new Pageable<Role>(Query, Page - 1, Size);
//            List<Role> Data = pageable.Data.ToList<Role>();

//            int TotalData = pageable.TotalCount;

//            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
//        }

//        public async override Task<int> CreateModel(Account account)
//        {          
//            AccountRoleService.Username = this.Username;

//            foreach (AccountRole accountRole in account.AccountRoles)
//            {
//                await AccountRoleService.CreateAsync(accountRole);
//            }

//            int Count = this.Create(account);

//            return Count;
//        }

//        public async override Task<int> UpdateModel(int Id, Account account)
//        {
//            int Count = 0;

//            using (var Transaction = this.DbContext.Database.BeginTransaction())
//            {
//                try
//                {
//                    this.DbContext.Entry(account).State = EntityState.Modified;

//                    if (string.IsNullOrWhiteSpace(account.Password))
//                    {
//                        this.DbContext.Entry(account).Property(p => p.Password).IsModified = false;
//                    }

//                    this.OnUpdating(Id, account);
//                    this.DbContext.AccountProfiles.Update(account.AccountProfile);
                    
//                    AccountRoleService.Username = this.Username;

//                    HashSet<int> AccountRoles = new HashSet<int>(this.DbContext.AccountRoles.Where(p => p.AccountId.Equals(Id)).Select(p => p.Id));

//                    foreach (int accountRole in AccountRoles)
//                    {
//                        AccountRole a = account.AccountRoles.FirstOrDefault(prop => prop.Id.Equals(accountRole));

//                        if (a == null)
//                        {
//                           await AccountRoleService.DeleteAsync(accountRole);
//                        }
//                    }

//                    foreach (AccountRole accountRole in account.AccountRoles)
//                    {
//                        if (accountRole.Id.Equals(0))
//                        {
//                            await AccountRoleService.CreateAsync(accountRole);
//                        }
//                    }

//                    Count = this.DbContext.SaveChanges();
//                    Transaction.Commit();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    Transaction.Rollback();
//                    throw;
//                }
//            }

//            return Count;
//        }

//        public async override Task<int> DeleteModel(int Id)
//        {
//            int Count = 0;

//            using (var Transaction = this.DbContext.Database.BeginTransaction())
//            {
//                try
//                {
//                    Count = this.Delete(Id);

                    
//                    AccountRoleService.Username = this.Username;

//                    HashSet<int> AccountRoles = new HashSet<int>(this.DbContext.AccountRoles.Where(p => p.AccountId.Equals(Id)).Select(p => p.Id));

//                    foreach (int accountRole in AccountRoles)
//                    {
//                        await AccountRoleService.DeleteAsync(accountRole);
//                    }

//                    Transaction.Commit();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    Transaction.Rollback();
//                    throw;
//                }
//            }

//            return Count;
//        }

//    }
//}
