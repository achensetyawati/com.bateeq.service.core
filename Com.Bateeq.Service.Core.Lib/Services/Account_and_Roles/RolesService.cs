//using Com.DanLiris.Service.Core.Lib.Helpers;
//using Com.DanLiris.Service.Core.Lib.Interfaces;
//using Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles;
//using Com.DanLiris.Service.Core.Lib.ViewModels;
//using Com.DanLiris.Service.Core.Lib.ViewModels.Account_and_Roles;
//using Com.Moonlay.NetCore.Lib;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;

//namespace Com.DanLiris.Service.Core.Lib.Services.Account_and_Roles
//{
//    public class RolesService : BasicService<CoreDbContext, Role>, IMap<Role, RoleViewModel>
//    {
//        private PermissionService permissionService;
//        public RolesService(IServiceProvider serviceProvider) : base(serviceProvider)
//        {
//            this.permissionService = this.ServiceProvider.GetService<PermissionService>();
//        }

//        public Role MapToModel(RoleViewModel roleVM)
//        {
//            Role role = new Role();
//            role.Permissions = new List<Permission>();

//            role.Code = roleVM.Code;
//            role.Name = roleVM.Name;
//            role.Description = roleVM.Description;

//            foreach (PermissionViewModel permissionVM in roleVM.Permissions)
//            {
//                Permission permission = new Permission();

//                permission.Id = permissionVM.Id;
//                permission._CreatedBy = permissionVM._CreatedBy;
//                permission._CreatedAgent = permissionVM._CreatedAgent;
//                permission._CreatedUtc = permissionVM._CreatedUtc;
//                permission.RoleId = permissionVM.RoleId;
//                permission.UnitId = permissionVM.unit.Id;
//                permission.Unit = permissionVM.unit.Name;
//                permission.UnitCode = permissionVM.unit.Code;
//                permission.permission = permissionVM.permission;
//                permission.Division = permissionVM.unit.Division != null ? permissionVM.unit.Division.Name : null;

//                role.Permissions.Add(permission);
//            }

//            return role;
//        }

//        public RoleViewModel MapToViewModel(Role role)
//        {
//            RoleViewModel roleVM = new RoleViewModel();
//            roleVM.Permissions = new List<PermissionViewModel>();

//            roleVM.Code = role.Code;
//            roleVM.Name = role.Name;
//            roleVM.Description = role.Description;

//            if (role.Permissions != null)
//            {
//                foreach (Permission permission in role.Permissions)
//                {
//                    PermissionViewModel permissionVM = new PermissionViewModel();
//                    permissionVM.unit = new UnitViewModel();
//                    permissionVM.unit.Division = new DivisionViewModel();

//                    permissionVM.Id = permission.Id;
//                    permissionVM._CreatedBy = permission._CreatedBy;
//                    permissionVM._CreatedAgent = permission._CreatedAgent;
//                    permissionVM._CreatedUtc = permission._CreatedUtc;
//                    permissionVM.RoleId = permission.RoleId;
//                    permissionVM.unit.Id = permission.UnitId;
//                    permissionVM.unit.Name = permission.Unit;
//                    permissionVM.unit.Code = permission.UnitCode;
//                    permissionVM.permission = permission.permission;
//                    permissionVM.unit.Division.Name = permission.Division;

//                    roleVM.Permissions.Add(permissionVM);
//                }
//            }

//            return roleVM;
//        }

//        public override Tuple<List<Role>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
//        {
//            IQueryable<Role> Query = this.DbContext.Roles;
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

//        public async override Task<Role> ReadModelById(int Id)
//        {
//            return await DbSet.Where(d => d.Id.Equals(Id)).Include(i => i.Permissions).FirstOrDefaultAsync();
//        }

//        public override Task<int> CreateModel(Role role)
//        {
//            //PermissionService permissionService = this.ServiceProvider.GetService<PermissionService>();
//            permissionService.Username = this.Username;

//            foreach (Permission data in role.Permissions)
//            {
//                this.permissionService.OnCreating(data);
//            }

//            int Count = this.Create(role);

//            return Task.FromResult(Count);
//        }

//        public async override Task<int> UpdateModel(int Id, Role role)
//        {
//            int Count = 0;

//            using (var Transaction = this.DbContext.Database.BeginTransaction())
//            {
//                try
//                {
//                    //PermissionService permissionService = ServiceProvider.GetService<PermissionService>();
//                    permissionService.Username = this.Username;
//                    HashSet<int> PermissionsData = new HashSet<int>(this.DbContext.Permissions.Where(p => p.RoleId.Equals(Id)).Select(p => p.Id));

//                    foreach (int permissionId in PermissionsData)
//                    {
//                        Permission p = role.Permissions.FirstOrDefault(prop => prop.Id.Equals(permissionId));

//                        if (p != null)
//                        {
//                            await permissionService.UpdateModel(p.Id, p);
//                        }
//                        else
//                        {
//                            await permissionService.DeleteModel(permissionId);
//                        }
//                    }

//                    foreach (Permission data in role.Permissions)
//                    {
//                        if (data.Id.Equals(0))
//                        {
//                            data.RoleId = Id;
//                            await permissionService.CreateModel(data);
//                        }
//                    }

//                    Count = this.Update(Id, role);
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

//        public override async Task<int> DeleteModel(int Id)
//        {
//            int Count = 0;
//            using (var Transaction = this.DbContext.Database.BeginTransaction())
//            {
//                try
//                {
//                    this.Delete(Id);

//                    PermissionService permissionService = this.ServiceProvider.GetService<PermissionService>();
//                    permissionService.Username = this.Username;
//                    HashSet<int> Permissions = new HashSet<int>(this.DbContext.Permissions.Where(p => p.RoleId.Equals(Id)).Select(p => p.Id));

//                    foreach (int id in Permissions)
//                    {
//                        await permissionService.DeleteModel(id);
//                    }

//                    Count = this.Delete(Id);
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

//        public override void OnCreating(Role model)
//        {
//            base.OnCreating(model);
//            model._CreatedAgent = "core-service";
//            model._CreatedBy = this.Username;
//        }

//        public override void OnUpdating(int id, Role model)
//        {
//            base.OnUpdating(id, model);
//            model._LastModifiedAgent = "core-service";
//            model._LastModifiedBy = this.Username;
//        }

//        public override void OnDeleting(Role model)
//        {
//            base.OnDeleting(model);
//            model._LastModifiedAgent = "core-service";
//            model._LastModifiedBy = this.Username;
//            model._DeletedAgent = "core-service";
//            model._DeletedBy = this.Username;
//        }
//    }
//}
