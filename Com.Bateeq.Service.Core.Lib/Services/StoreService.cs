using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class StoreService : BasicService<CoreDbContext, MasterStore>, IMap<MasterStore, StoreViewModel>
    {
        public StoreService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<MasterStore>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MasterStore> Query = this.DbContext.MasterStores;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                     "Code","Name","Address","City"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                 "Id","code", "name", "description","address","city","closedDate","monthlyTotalCost","OnlineOffline","openedDate",
                 "Pic","Phone","salesCapital","SalesCategory","salesTarget","status","storeArea","StoreCategory","storeWide","Longitude","Latitude"
            };

            Query = Query
                .Select(b => new MasterStore
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Address = b.Address,
                    StoreCategory = b.StoreCategory,
                    OnlineOffline = b.OnlineOffline,
                    Status = b.Status,
                    SalesCategory = b.SalesCategory,
                    Longitude = b.Longitude,
                    Latitude = b.Latitude,
                    Pic = b.Pic,
                    Phone = b.Phone,
                    City=b.City
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
            Pageable<MasterStore> pageable = new Pageable<MasterStore>(Query, Page - 1, Size);
            List<MasterStore> Data = pageable.Data.ToList<MasterStore>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }
        public StoreViewModel MapToViewModel(MasterStore season)
        {
            StoreViewModel seasonVM = new StoreViewModel();

            seasonVM.Id = season.Id;
            seasonVM.UId = season.Uid;
            seasonVM._IsDeleted = season._IsDeleted;
            seasonVM.Active = season.Active;
            seasonVM._CreatedUtc = season._CreatedUtc;
            seasonVM._CreatedBy = season._CreatedBy;
            seasonVM._CreatedAgent = season._CreatedAgent;
            seasonVM._LastModifiedUtc = season._LastModifiedUtc;
            seasonVM._LastModifiedBy = season._LastModifiedBy;
            seasonVM._LastModifiedAgent = season._LastModifiedAgent;
            seasonVM.code = season.Code;
            seasonVM.name = season.Name;
            seasonVM.address = season.Address;
            seasonVM.city = season.City;
            seasonVM.closedDate = season.ClosedDate;
            seasonVM.description = season.Description;
            seasonVM.monthlyTotalCost = season.MonthlyTotalCost;
            seasonVM.onlineOffline = season.OnlineOffline;
            seasonVM.openedDate = season.OpenedDate;
            seasonVM.phone = season.Phone;
            seasonVM.pic = season.Pic;
            seasonVM.salesCapital = season.SalesCapital;
            seasonVM.salesCategory = season.SalesCategory;
            seasonVM.salesTarget = season.SalesTarget;
            seasonVM.status = season.Status;
            seasonVM.storeArea = season.StoreArea;
            seasonVM.storeCategory = season.StoreCategory;
            seasonVM.storeWide = season.StoreWide;
            seasonVM.longitude = season.Longitude;
            seasonVM.latitude = season.Latitude;

            return seasonVM;
        }

        public MasterStore MapToModel(StoreViewModel seasonVM)
        {
            MasterStore season = new MasterStore();

            season.Id = seasonVM.Id;
            season.Uid = seasonVM.UId;
            season._IsDeleted = seasonVM._IsDeleted;
            season.Active = seasonVM.Active;
            season._CreatedUtc = seasonVM._CreatedUtc;
            season._CreatedBy = seasonVM._CreatedBy;
            season._CreatedAgent = seasonVM._CreatedAgent;
            season._LastModifiedUtc = seasonVM._LastModifiedUtc;
            season._LastModifiedBy = seasonVM._LastModifiedBy;
            season._LastModifiedAgent = seasonVM._LastModifiedAgent;
            season.Code = seasonVM.code;
            season.Name = seasonVM.name;
            season.Address = seasonVM.address;
            season.City = seasonVM.city;
            season.ClosedDate = seasonVM.closedDate;
            season.Description = seasonVM.description;
            season.MonthlyTotalCost = seasonVM.monthlyTotalCost;
            season.OnlineOffline = seasonVM.onlineOffline;
            season.OpenedDate = seasonVM.openedDate;
            season.Phone = seasonVM.phone;
            season.Pic = seasonVM.pic;
            season.SalesCapital = seasonVM.salesCapital;
            season.SalesCategory = seasonVM.salesCategory;
            season.SalesTarget = seasonVM.salesTarget;
            season.Status = seasonVM.status;
            season.StoreArea = seasonVM.storeArea;
            season.StoreCategory = seasonVM.storeCategory;
            season.StoreWide = seasonVM.storeWide;
            season.Longitude = seasonVM.longitude;
            season.Latitude = seasonVM.latitude;

            return season;
        }

        public sealed class MaterialMap : ClassMap<StoreViewModel>
        {
            public MaterialMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
            }
        }

        public Task<List<MasterStore>> GetStoreByCategory(string category)
        {
            var store = (from a in DbContext.MasterStores
                         where a.StoreCategory.Contains((string.IsNullOrWhiteSpace(category) ? a.StoreCategory : category))
                         select a).ToListAsync();
            return store;
        }


        public Task<MasterStore> GetStoreByCode(string code)
        {
            var store = (from a in DbContext.MasterStores
                         where a.Code.Contains((string.IsNullOrWhiteSpace(code) ? a.Code : code))
                         select a).FirstOrDefaultAsync();
            return store;
        }

        public Task<List<MasterStore>> GetNearestStoreByCode(string code)
        {
            var store = (from a in DbContext.MasterStores
                         where a.Code == code
                         select a).Single();
            var nearest= (from x in DbContext.MasterStores
                          where x.City.Contains(store.City)
                          select x).ToListAsync();
            return nearest;
        }

        public Task<StoreStorageViewModel> GetStoreStorageByCode(string code)
        {
            var store = (from a in DbContext.MasterStores
                         join b in DbContext.Storages on a.Code equals b.Code
                         where a.Code.Contains((string.IsNullOrWhiteSpace(code) ? a.Code : code))
                         select new StoreStorageViewModel
                         {
                             address = a.Address,
                             city = a.City,
                             closedDate = a.ClosedDate,
                             code = a.Code,
                             description = a.Description,
                             Id = a.Id,
                             monthlyTotalCost = a.MonthlyTotalCost,
                             name = a.Name,
                             onlineOffline = a.OnlineOffline,
                             openedDate = a.OpenedDate,
                             phone = a.Phone,
                             pic = a.Pic,
                             salesCapital = a.SalesCapital,
                             salesCategory = a.SalesCategory,
                             salesTarget = a.SalesTarget,
                             status = a.Status,
                             storeArea = a.StoreArea,
                             storeCategory = a.StoreCategory,
                             storeWide = a.StoreWide,
                             storage = new StorageViewModel
                             {
                                 address = b.Address,
                                 code = b.Code,
                                 description = b.Description,
                                 isCentral = b.IsCentral,
                                 name = b.Name,
                                 phone = b.Phone,
                                 _id = b.Id
                             }

                         }).FirstOrDefaultAsync();
            return store;
        }
    }

}
