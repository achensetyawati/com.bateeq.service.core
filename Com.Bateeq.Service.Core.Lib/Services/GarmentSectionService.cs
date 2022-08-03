using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class GarmentSectionService : BasicService<CoreDbContext, GarmentSection>, IMap<GarmentSection, GarmentSectionViewModel>
    {
        public GarmentSectionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public GarmentSection MapToModel(GarmentSectionViewModel viewModel)
        {
            GarmentSection model = new GarmentSection();
            PropertyCopier<GarmentSectionViewModel, GarmentSection>.Copy(viewModel, model);
            return model;
        }
        
        public GarmentSectionViewModel MapToViewModel(GarmentSection model)
        {
            GarmentSectionViewModel viewModel = new GarmentSectionViewModel();
            PropertyCopier<GarmentSection, GarmentSectionViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public override Tuple<List<GarmentSection>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentSection> Query = this.DbContext.GarmentSections;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Name", "Remark", "_LastModifiedUtc"
            };

            Query = Query
                .Select(b => new GarmentSection
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Remark = b.Remark,
                    _LastModifiedUtc = b._LastModifiedUtc
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
            Pageable<GarmentSection> pageable = new Pageable<GarmentSection>(Query, Page - 1, Size);
            List<GarmentSection> Data = pageable.Data.ToList<GarmentSection>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }
    }
}
