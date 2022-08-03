using Com.DanLiris.Service.Core.Lib.Models.Module;
using Com.Moonlay.NetCore.Lib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using CsvHelper.Configuration;
using System.Dynamic;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Microsoft.Extensions.Primitives;
using Com.DanLiris.Service.Core.Lib.ViewModels.Module;
using Module = Com.DanLiris.Service.Core.Lib.Models.Module.Module;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ModuleService : BasicService<CoreDbContext, Module>, IMap<Module, ModuleViewModel>
    {
        public ModuleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



       // public IQueryable<ModuleViewModel> GetReportQuery(string keyword)
       // {
       
       //     var Query1 = (from a in DbContext.Modules
       //                  join b in DbContext.ModuleDesstinations on a.Id equals b.ModuleId


       //                  select new ModuleViewModel
       //                  {
       //                      UId =a.UId,
       //                      Code = a.Code,
       //                      Description = a.Description,
       //                      Name   = a.Name,
       //});

       //     var Query2 = (from a in DbContext.Modules
       //                   join b in DbContext.ModuleSources on a.Id equals b.ModuleId


       //                   select new ModuleViewModel
       //                   {
       //                       UId = a.UId,
       //                       Code = a.Code,
       //                       Description = a.Description,
       //                       Name = a.Name,
       //                   });

       //     var Query3 = (from a in Query1
       //                  from b in Query2

       //                  select new ModuleViewModel
       //                  {

       //                  });
       //     Dictionary<string, double> q = new Dictionary<string, double>();
       //     List<UnitReceiptNoteReportViewModel> urn = new List<UnitReceiptNoteReportViewModel>();
       //     foreach (UnitReceiptNoteReportViewModel data in Query.ToList())
       //     {
       //         double value;
       //         if (q.TryGetValue(data.productCode + data.prNo + data.epoDetailId.ToString(), out value))
       //         {
       //             q[data.productCode + data.prNo + data.epoDetailId.ToString()] -= data.receiptQuantity;
       //             data.quantity = q[data.productCode + data.prNo + data.epoDetailId.ToString()];
       //             urn.Add(data);
       //         }
       //         else
       //         {
       //             q[data.productCode + data.prNo + data.epoDetailId.ToString()] = data.quantity - data.receiptQuantity;
       //             data.quantity = q[data.productCode + data.prNo + data.epoDetailId.ToString()];
       //             urn.Add(data);
       //         }

       //     }
       //     return Query = urn.AsQueryable();
       // }



        public override Tuple<List<Module>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Module> Query = this.DbContext.Modules;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                     "Code","Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                 "_id", "Code", "Name", "Description"
            };

            Query = Query
                .Select(b => new Module
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name
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
            Pageable<Module> pageable = new Pageable<Module>(Query, Page - 1, Size);
            List<Module> Data = pageable.Data.ToList<Module>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ModuleViewModel MapToViewModel(Module module)
        {
            ModuleViewModel moduleVM = new ModuleViewModel();

            moduleVM._id = module.Id;
            moduleVM.UId = module.UId;
            moduleVM._deleted = module._IsDeleted;
            moduleVM._active = module.Active;
            moduleVM._createdDate = module._CreatedUtc;
            moduleVM._createdBy = module._CreatedBy;
            moduleVM._createAgent = module._CreatedAgent;
            moduleVM._updatedDate = module._LastModifiedUtc;
            moduleVM._updatedBy = module._LastModifiedBy;
            moduleVM._updateAgent = module._LastModifiedAgent;
            moduleVM.Code = module.Code;
            moduleVM.Name = module.Name;


            return moduleVM;
        }

        public Module MapToModel(ModuleViewModel moduleVM)
        {
            Module module = new Module();

            module.Id = moduleVM._id;
            module.UId = moduleVM.UId;
            module._IsDeleted = moduleVM._deleted;
            module.Active = moduleVM._active;
            module._CreatedUtc = moduleVM._createdDate;
            module._CreatedBy = moduleVM._createdBy;
            module._CreatedAgent = moduleVM._createAgent;
            module._LastModifiedUtc = moduleVM._updatedDate;
            module._LastModifiedBy = moduleVM._updatedBy;
            module._LastModifiedAgent = moduleVM._updateAgent;
            module.Code = moduleVM.Code;
            module.Name = moduleVM.Name;

            return module;
        }




    }
}
