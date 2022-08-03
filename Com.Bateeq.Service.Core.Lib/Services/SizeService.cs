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
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class SizeService : BasicService<CoreDbContext, SizeModel>, IMap<SizeModel, SizeViewModel>
    {
        public SizeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public SizeModel MapToModel(SizeViewModel viewModel)
        {
            SizeModel model = new SizeModel();
            PropertyCopier<SizeViewModel, SizeModel>.Copy(viewModel, model);
            return model;
        }

        public SizeViewModel MapToViewModel(SizeModel model)
        {
            SizeViewModel viewModel = new SizeViewModel();
            PropertyCopier<SizeModel, SizeViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public override Tuple<List<SizeModel>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<SizeModel> Query = this.DbContext.Sizes;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Size", 
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Size", "_LastModifiedUtc"
            };

            Query = Query
                .Select(b => new SizeModel
                {
                    Id = b.Id,
                    Size = b.Size,
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
            Pageable<SizeModel> pageable = new Pageable<SizeModel>(Query, Page - 1, Size);
            List<SizeModel> Data = pageable.Data.ToList<SizeModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }
    }
}