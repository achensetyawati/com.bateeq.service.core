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
    public class ArticleSeasonService : BasicService<CoreDbContext, ArticleSeason>, IMap<ArticleSeason, ArticleSeasonViewModel>
    {
        public ArticleSeasonService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



        public override Tuple<List<ArticleSeason>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleSeason> Query = this.DbContext.ArticleSeasons;
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
                .Select(b => new ArticleSeason
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
            Pageable<ArticleSeason> pageable = new Pageable<ArticleSeason>(Query, Page - 1, Size);
            List<ArticleSeason> Data = pageable.Data.ToList<ArticleSeason>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleSeasonViewModel MapToViewModel(ArticleSeason season)
        {
            ArticleSeasonViewModel seasonVM = new ArticleSeasonViewModel();

            seasonVM._id = season.Id;
            seasonVM.UId = season.UId;
            seasonVM._deleted = season._IsDeleted;
            seasonVM._active = season.Active;
            seasonVM._createdDate = season._CreatedUtc;
            seasonVM._createdBy = season._CreatedBy;
            seasonVM._createAgent = season._CreatedAgent;
            seasonVM._updatedDate = season._LastModifiedUtc;
            seasonVM._updatedBy = season._LastModifiedBy;
            seasonVM._updateAgent = season._LastModifiedAgent;
            seasonVM.code = season.Code;
            seasonVM.name = season.Name;


            return seasonVM;
        }

        public ArticleSeason MapToModel(ArticleSeasonViewModel seasonVM)
        {
            ArticleSeason season = new ArticleSeason();

            season.Id = seasonVM._id;
            season.UId = seasonVM.UId;
            season._IsDeleted = seasonVM._deleted;
            season.Active = seasonVM._active;
            season._CreatedUtc = seasonVM._createdDate;
            season._CreatedBy = seasonVM._createdBy;
            season._CreatedAgent = seasonVM._createAgent;
            season._LastModifiedUtc = seasonVM._updatedDate;
            season._LastModifiedBy = seasonVM._updatedBy;
            season._LastModifiedAgent = seasonVM._updateAgent;
            season.Code = seasonVM.code;
            season.Name = seasonVM.name;

            return season;
        }



        public sealed class MaterialMap : ClassMap<ArticleSeasonViewModel>
        {
            public MaterialMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
            }
        }
    }
}
