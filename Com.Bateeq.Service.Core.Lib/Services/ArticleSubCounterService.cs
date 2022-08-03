using Com.DanLiris.Service.Core.Lib.Models;
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

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ArticleSubCounterService : BasicService<CoreDbContext, ArticleSubCounter>, IMap<ArticleSubCounter, ArticleSubCounterViewModel>
    {
        public ArticleSubCounterService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



        public override Tuple<List<ArticleSubCounter>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleSubCounter> Query = this.DbContext.ArticleSubCounters;
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
                 "_id","Code", "Name", "description"
            };

            Query = Query
                .Select(b => new ArticleSubCounter
                {
                    Id   = b.Id,
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
            Pageable<ArticleSubCounter> pageable = new Pageable<ArticleSubCounter>(Query, Page - 1, Size);
            List<ArticleSubCounter> Data = pageable.Data.ToList<ArticleSubCounter>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleSubCounterViewModel MapToViewModel(ArticleSubCounter subCounter)
        {
            ArticleSubCounterViewModel subCounterVM = new ArticleSubCounterViewModel();

            subCounterVM._id = subCounter.Id;
            subCounterVM.UId = subCounter.UId;
            subCounterVM._deleted = subCounter._IsDeleted;
            subCounterVM._active = subCounter.Active;
            subCounterVM._createdDate = subCounter._CreatedUtc;
            subCounterVM._createdBy = subCounter._CreatedBy;
            subCounterVM._createAgent = subCounter._CreatedAgent;
            subCounterVM._updatedDate = subCounter._LastModifiedUtc;
            subCounterVM._updatedBy = subCounter._LastModifiedBy;
            subCounterVM._updateAgent = subCounter._LastModifiedAgent;
            subCounterVM.code = subCounter.Code;
            subCounterVM.name = subCounter.Name;


            return subCounterVM;
        }

        public ArticleSubCounter MapToModel(ArticleSubCounterViewModel subCounterVM)
        {
            ArticleSubCounter subCounter = new ArticleSubCounter();

            subCounter.Id = subCounterVM._id;
            subCounter.UId = subCounterVM.UId;
            subCounter._IsDeleted = subCounterVM._deleted;
            subCounter.Active = subCounterVM._active;
            subCounter._CreatedUtc = subCounterVM._createdDate;
            subCounter._CreatedBy = subCounterVM._createdBy;
            subCounter._CreatedAgent = subCounterVM._createAgent;
            subCounter._LastModifiedUtc = subCounterVM._updatedDate;
            subCounter._LastModifiedBy = subCounterVM._updatedBy;
            subCounter._LastModifiedAgent = subCounterVM._updateAgent;
            subCounter.Code = subCounterVM.code;
            subCounter.Name = subCounterVM.name;

            return subCounter;
        }



        public sealed class SubCounterMap : ClassMap<ArticleSubCounterViewModel>
        {
            public SubCounterMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
            }
        }
    }
}
