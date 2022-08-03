using Com.DanLiris.Service.Core.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Com.DanLiris.Service.Core.Lib.ViewModels.Module;
using Com.DanLiris.Service.Core.Lib.Models.Module;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class StorageService : BasicService<CoreDbContext, Storage>, IMap<Storage, StorageViewModel>
    {
        public StorageService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Storage>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Storage> Query = this.DbContext.Storages;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name", "UnitName","Description"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "_id", "code", "name", "unit","description"
            };

            Query = Query
                .Select(s => new Storage
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    UnitName = s.UnitName,
                    DivisionName = s.DivisionName,
                    Description=s.Description
                });

            /* Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_updatedDate", General.DESCENDING);

                Query = Query.OrderByDescending(b => b._LastModifiedUtc); /* Default Order */
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                string TransformKey = General.TransformOrderBy(Key);

                BindingFlags IgnoreCase = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

                Query = OrderType.Equals(General.ASCENDING) ?
                    Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
                    Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
            }

            /* Pagination */
            Pageable<Storage> pageable = new Pageable<Storage>(Query, Page - 1, Size);
            List<Storage> Data = pageable.Data.ToList<Storage>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }


        public Tuple<List<StorageViewModel>, int, Dictionary<string, string>> GetDestination(int Page = 1, int Size = 25, string Order = "{}", List<string> select = null, string Keyword = null, string Filter = "{}")
        {


            IQueryable<StorageViewModel> Query = (from a in DbContext.Modules
                                                  join b in DbContext.ModuleDestinations on a.Id equals b.ModuleId
                                                  join c in DbContext.Storages on b.DestinationValue equals c.UId

                                                  where
                                                  a.Code == Keyword

                                                  select new StorageViewModel
                                                  {
                                                      code = c.Code,
                                                      name = c.Name,
                                                      UId = c.UId,
                                                      _id = c.Id
                                                  }

                ).AsQueryable();



            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            //Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);



            //List<string> SelectedFields = new List<string>()
            //{
            //    "CorrectionNo", "CorrectionType", "SupplierName", "DONo"
            //};





            /* Pagination */
            Pageable<StorageViewModel> pageable = new Pageable<StorageViewModel>(Query, Page - 1, Size);
            List<StorageViewModel> Data = pageable.Data.ToList<StorageViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<StorageViewModel>, int, Dictionary<string, string>> GetSource(int Page = 1, int Size = 25, string Order = "{}", List<string> select = null, string Keyword = null, string Filter = "{}")
        {


            IQueryable<StorageViewModel> Query = (from a in DbContext.Modules
                                                  join b in DbContext.ModuleSources on a.Id equals b.ModuleId
                                                  join c in DbContext.Storages on b.SourceValue equals c.UId

                                                  where
                                                  a.Code == Keyword

                                                  select new StorageViewModel
                                                  {
                                                      code = c.Code,
                                                      name = c.Name,
                                                      UId = c.UId,
                                                      _id = c.Id
                                                  }

                ).AsQueryable();



            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            //Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);



            //List<string> SelectedFields = new List<string>()
            //{
            //    "CorrectionNo", "CorrectionType", "SupplierName", "DONo"
            //};





            /* Pagination */
            Pageable<StorageViewModel> pageable = new Pageable<StorageViewModel>(Query, Page - 1, Size);
            List<StorageViewModel> Data = pageable.Data.ToList<StorageViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public StorageViewModel MapToViewModel(Storage storage)
        {
            StorageViewModel storageVM = new StorageViewModel();
            storageVM.unit = new StorageUnitViewModel();
            storageVM.unit.division = new DivisionViewModel();

            storageVM._id = storage.Id;
            storageVM.UId = storage.UId;
            storageVM._deleted = storage._IsDeleted;
            storageVM._active = storage.Active;
            storageVM._createdDate = storage._CreatedUtc;
            storageVM._createdBy = storage._CreatedBy;
            storageVM._createAgent = storage._CreatedAgent;
            storageVM._updatedDate = storage._LastModifiedUtc;
            storageVM._updatedBy = storage._LastModifiedBy;
            storageVM._updateAgent = storage._LastModifiedAgent;
            storageVM.code = storage.Code;
            storageVM.name = storage.Name;
            storageVM.description = storage.Description;
            storageVM.unit._id = storage.UnitId;
            storageVM.unit.name = storage.UnitName;
            storageVM.unit.division.Name = storage.DivisionName;
            storageVM.code = storage.Code;
            storageVM.address = storage.Address;
            storageVM.phone = storage.Phone;

            var ModuleSources = (from a in DbContext.Modules
                           join b in DbContext.ModuleSources on a.Id equals b.ModuleId
                           join c in DbContext.Storages on b.StorageId equals c.Id
                           where b.SourceValue == storageVM.UId
                           select new ModuleSourceViewModel
                           {
                               _id=b.Id,
                               moduleSource=new ModuleViewModel
                               {
                                   Name = a.Name,
                                   Code = a.Code,
                                   _id = a.Id
                               }
                           });
            storageVM.moduleSources = ModuleSources.ToList();
            var ModuleDestinations = (from a in DbContext.Modules
                                 join b in DbContext.ModuleDestinations on a.Id equals b.ModuleId
                                 join c in DbContext.Storages on b.StorageId equals c.Id
                                 where b.DestinationValue == storageVM.UId
                                 select new ModuleDestinationViewModel
                                 {
                                     _id=b.Id,
                                     moduleDestination= new ModuleViewModel
                                     {
                                         Name = a.Name,
                                         Code = a.Code,
                                         _id = a.Id
                                     }
                                 });
            storageVM.moduleDestinations = ModuleDestinations.ToList();

            return storageVM;
        }

        public Storage MapToModel(StorageViewModel storageVM)
        {
            Storage storage = new Storage();

            storage.Id = storageVM._id;
            storage.UId = storageVM.code;
            storage._IsDeleted = storageVM._deleted;
            storage.Active = storageVM._active;
            storage._CreatedUtc = storageVM._createdDate;
            storage._CreatedBy = storageVM._createdBy;
            storage._CreatedAgent = storageVM._createAgent;
            storage._LastModifiedUtc = storageVM._updatedDate;
            storage._LastModifiedBy = storageVM._updatedBy;
            storage._LastModifiedAgent = storageVM._updateAgent;
            storage.Code = storageVM.code;
            storage.Name = storageVM.name;
            storage.Description = storageVM.description;
            storage.Address = storageVM.address;
            storage.Phone = storageVM.phone;

            if (!Equals(storageVM.unit, null))
            {
                storage.UnitId = storageVM.unit._id;
                storage.UnitName = storageVM.unit.name;
                storage.DivisionName = storageVM.unit.division.Name;
            }
            else
            {
                storage.UnitId = null;
                storage.UnitName = null;
                storage.DivisionName = null;
            }

            storage.ModuleSources = new List<ModuleSource>();
            if (storageVM.moduleSources!=null || storageVM.moduleSources.Count != 0)
            {
                foreach (var s in storageVM.moduleSources)
                {
                    ModuleSource ms = new ModuleSource
                    {
                        Id=s._id,
                        ModuleId = s.moduleSource._id,
                        SourceValue = storage.Code
                    };
                    storage.ModuleSources.Add(ms);
                }
            }

            storage.ModuleDestinations = new List<ModuleDestination>();
            if (storageVM.moduleDestinations != null || storageVM.moduleDestinations.Count != 0)
            {
                foreach (var d in storageVM.moduleDestinations)
                {
                    ModuleDestination md = new ModuleDestination
                    {
                        Id=d._id,
                        ModuleId = d.moduleDestination._id,
                        DestinationValue = storage.Code
                    };
                    storage.ModuleDestinations.Add(md);
                }
            }
                

            return storage;
        }

        public override void OnCreating(Storage model)
        {
            //CodeGenerator codeGenerator = new CodeGenerator();

            //do
            //{
            //    model.Code = codeGenerator.GenerateCode();
            //}
            //while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

            base.OnCreating(model);
        }


        public Task<List<StorageByNameViewModel>> GetStorageByName(string keyword, int page, int size)
        {
            var query = DbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(entity => entity.Name.Contains(keyword));
            }

            return query.Skip((page - 1) * size).Take(size).Select(entity => new StorageByNameViewModel()
            {
                Code = entity.Code,
                Id = entity.Id,
                Name = entity.Name,
                Unit = new UnitStorage()
                {
                    Division = new DivisionStorage()
                    {
                        Name = entity.DivisionName
                    },
                    Name = entity.UnitName
                }
            }).ToListAsync();
        }

        public Task<List<StorageViewModel>> GetStorage1(string storageId)
        {

            var storage = DbContext.Storages.Where(x => x.UId == storageId);
            return storage.Select(x => new StorageViewModel()
            {
                _id = x.Id,
                code = x.Code,
                name = x.Name
            }).ToListAsync();
        }

        public Storage GetStoragebyCode(string Code)
        {
            var storage = DbSet.Where(x => x.Code == Code).FirstOrDefault();
            return storage;
        }


        public override async Task<int> UpdateModel(int Id, Storage Model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldM = this.DbSet
                        .Include(d => d.ModuleSources)
                        .Include(d=>d.ModuleDestinations)
                        .SingleOrDefault(s => s.Id == Id);

                    if (oldM != null && oldM.Id == Id)
                    {
                        foreach (var oldSource in oldM.ModuleSources)
                        {
                            var newItem = Model.ModuleSources.FirstOrDefault(i => i.Id.Equals(oldSource.Id));
                            if (newItem == null)
                            {
                                oldSource._IsDeleted = true;
                                oldSource._DeletedUtc = DateTime.Now;
                                oldSource._DeletedBy = Model._LastModifiedBy;
                                oldSource._DeletedAgent = Model._LastModifiedAgent;
                            }
                        }

                        foreach (var item in Model.ModuleSources.Where(i => i.Id == 0))
                        {
                            oldM.ModuleSources.Add(item);
                        }

                        foreach (var oldDestination in oldM.ModuleDestinations)
                        {
                            var newItem = Model.ModuleDestinations.FirstOrDefault(i => i.Id.Equals(oldDestination.Id));
                            if (newItem == null)
                            {
                                oldDestination._IsDeleted = true;
                                oldDestination._DeletedUtc = DateTime.Now;
                                oldDestination._DeletedBy = Model._LastModifiedBy;
                                oldDestination._DeletedAgent = Model._LastModifiedAgent;
                            }
                        }


                        foreach (var item in Model.ModuleDestinations.Where(i => i.Id == 0))
                        {
                            oldM.ModuleDestinations.Add(item);
                        }

                        oldM.Address = Model.Address;
                        oldM.Phone = Model.Phone;
                        oldM.UnitName = Model.UnitName;
                        oldM.UnitId = Model.UnitId;
                        oldM.DivisionName = Model.DivisionName;
                        oldM.Description = Model.Description;

                        Updated = await DbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception("Invalid Id");
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Updated;
        }
    }
}