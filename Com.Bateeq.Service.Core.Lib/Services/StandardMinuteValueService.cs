using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class StandardMinuteValueService : BasicService<CoreDbContext, StandardMinuteValue>, IMap<StandardMinuteValue, StandardMinuteValueViewModel>
    {
        public StandardMinuteValueService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public StandardMinuteValue MapToModel(StandardMinuteValueViewModel viewModel)
        {
            StandardMinuteValue model = new StandardMinuteValue();
            PropertyCopier<StandardMinuteValueViewModel, StandardMinuteValue>.Copy(viewModel, model);
            return model;
        }

        public StandardMinuteValueViewModel MapToViewModel(StandardMinuteValue model)
        {
            StandardMinuteValueViewModel viewModel = new StandardMinuteValueViewModel();
            PropertyCopier<StandardMinuteValue, StandardMinuteValueViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public override Tuple<List<StandardMinuteValue>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<StandardMinuteValue> Query = this.DbContext.StandardMinuteValues;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "BuyerName", "ComodityName"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "BuyerName", "ComodityName", "SMVCutting", "SMVSewing", "SMVFinishing", "_LastModifiedUtc"
            };

            Query = Query
                .Select(b => new StandardMinuteValue
                {
                    Id = b.Id,
                    BuyerId = b.BuyerId,
                    BuyerCode = b.BuyerCode,
                    BuyerName = b.BuyerName,
                    ComodityId = b.ComodityId,
                    ComodityName = b.ComodityName,
                    ComodityCode = b.ComodityCode,
                    SMVDate = b.SMVDate,
                    SMVCutting = b.SMVCutting,
                    SMVFinishing = b.SMVFinishing,
                    SMVSewing = b.SMVSewing,
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
            Pageable<StandardMinuteValue> pageable = new Pageable<StandardMinuteValue>(Query, Page - 1, Size);
            List<StandardMinuteValue> Data = pageable.Data.ToList<StandardMinuteValue>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }
    }
}
