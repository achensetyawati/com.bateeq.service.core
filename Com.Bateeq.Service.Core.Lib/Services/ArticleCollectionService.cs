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
    public class ArticleCollectionService : BasicService<CoreDbContext, ArticleCollection>, IMap<ArticleCollection, ArticleCollectionViewModel>
    {
        public ArticleCollectionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        public override Tuple<List<ArticleCollection>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleCollection> Query = this.DbContext.ArticleCollections;
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
                .Select(b => new ArticleCollection
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
            Pageable<ArticleCollection> pageable = new Pageable<ArticleCollection>(Query, Page - 1, Size);
            List<ArticleCollection> Data = pageable.Data.ToList<ArticleCollection>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleCollectionViewModel MapToViewModel(ArticleCollection collection)
        {
            ArticleCollectionViewModel collectionVM = new ArticleCollectionViewModel();

            collectionVM._id = collection.Id;
            collectionVM.UId = collection.UId;
            collectionVM._deleted = collection._IsDeleted;
            collectionVM._active = collection.Active;
            collectionVM._createdDate = collection._CreatedUtc;
            collectionVM._createdBy = collection._CreatedBy;
            collectionVM._createAgent = collection._CreatedAgent;
            collectionVM._updatedDate = collection._LastModifiedUtc;
            collectionVM._updatedBy = collection._LastModifiedBy;
            collectionVM._updateAgent = collection._LastModifiedAgent;
            collectionVM.code = collection.Code;
            collectionVM.name = collection.Name;


            return collectionVM;
        }

        public ArticleCollection MapToModel(ArticleCollectionViewModel collectionVM)
        {
            ArticleCollection collection = new ArticleCollection();

            collection.Id = collectionVM._id;
            collection.UId = collectionVM.UId;
            collection._IsDeleted = collectionVM._deleted;
            collection.Active = collectionVM._active;
            collection._CreatedUtc = collectionVM._createdDate;
            collection._CreatedBy = collectionVM._createdBy;
            collection._CreatedAgent = collectionVM._createAgent;
            collection._LastModifiedUtc = collectionVM._updatedDate;
            collection._LastModifiedBy = collectionVM._updatedBy;
            collection._LastModifiedAgent = collectionVM._updateAgent;
            collection.Code = collectionVM.code;
            collection.Name = collectionVM.name;

            return collection;
        }

    }
}
