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
    public class ArticleCounterService : BasicService<CoreDbContext, ArticleCounter>, IMap<ArticleCounter, ArticleCounterViewModel>
    {
        public ArticleCounterService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



        public override Tuple<List<ArticleCounter>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleCounter> Query = this.DbContext.ArticleCounters;
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
                .Select(b => new ArticleCounter
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
            Pageable<ArticleCounter> pageable = new Pageable<ArticleCounter>(Query, Page - 1, Size);
            List<ArticleCounter> Data = pageable.Data.ToList<ArticleCounter>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleCounterViewModel MapToViewModel(ArticleCounter counter)
        {
            ArticleCounterViewModel counterVM = new ArticleCounterViewModel();

            counterVM._id = counter.Id;
            counterVM.UId = counter.UId;
            counterVM._deleted = counter._IsDeleted;
            counterVM._active = counter.Active;
            counterVM._createdDate = counter._CreatedUtc;
            counterVM._createdBy = counter._CreatedBy;
            counterVM._createAgent = counter._CreatedAgent;
            counterVM._updatedDate = counter._LastModifiedUtc;
            counterVM._updatedBy = counter._LastModifiedBy;
            counterVM._updateAgent = counter._LastModifiedAgent;
            counterVM.code = counter.Code;
            counterVM.name = counter.Name;


            return counterVM;
        }

        public ArticleCounter MapToModel(ArticleCounterViewModel counterVM)
        {
            ArticleCounter counter = new ArticleCounter();

            counter.Id = counterVM._id;
            counter.UId = counterVM.UId;
            counter._IsDeleted = counterVM._deleted;
            counter.Active = counterVM._active;
            counter._CreatedUtc = counterVM._createdDate;
            counter._CreatedBy = counterVM._createdBy;
            counter._CreatedAgent = counterVM._createAgent;
            counter._LastModifiedUtc = counterVM._updatedDate;
            counter._LastModifiedBy = counterVM._updatedBy;
            counter._LastModifiedAgent = counterVM._updateAgent;
            counter.Code = counterVM.code;
            counter.Name = counterVM.name;

            return counter;
        }



        public sealed class CounterMap : ClassMap<ArticleCounterViewModel>
        {
            public CounterMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
            }
        }
    }
}
