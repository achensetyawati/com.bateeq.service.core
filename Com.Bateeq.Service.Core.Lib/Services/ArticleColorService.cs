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
    public class ArticleColorService : BasicService<CoreDbContext, ArticleColor>, IMap<ArticleColor, ArticleColorViewModel>
    {
        public ArticleColorService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<ArticleColor>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleColor> Query = this.DbContext.ArticleColors;
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
                .Select(b => new ArticleColor
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
            Pageable<ArticleColor> pageable = new Pageable<ArticleColor>(Query, Page - 1, Size);
            List<ArticleColor> Data = pageable.Data.ToList<ArticleColor>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }


        public ArticleColorViewModel MapToViewModel(ArticleColor color)
        {
            ArticleColorViewModel colorVM = new ArticleColorViewModel();

            colorVM._id = color.Id;
            colorVM.UId = color.UId;
            colorVM._deleted = color._IsDeleted;
            colorVM._active = color.Active;
            colorVM._createdDate = color._CreatedUtc;
            colorVM._createdBy = color._CreatedBy;
            colorVM._createAgent = color._CreatedAgent;
            colorVM._updatedDate = color._LastModifiedUtc;
            colorVM._updatedBy = color._LastModifiedBy;
            colorVM._updateAgent = color._LastModifiedAgent;
            colorVM.code = color.Code;
            colorVM.name = color.Name;


            return colorVM;
        }

        public ArticleColor MapToModel(ArticleColorViewModel colorVM)
        {
            ArticleColor color = new ArticleColor();

            color.Id = colorVM._id;
            color.UId = colorVM.UId;
            color._IsDeleted = colorVM._deleted;
            color.Active = colorVM._active;
            color._CreatedUtc = colorVM._createdDate;
            color._CreatedBy = colorVM._createdBy;
            color._CreatedAgent = colorVM._createAgent;
            color._LastModifiedUtc = colorVM._updatedDate;
            color._LastModifiedBy = colorVM._updatedBy;
            color._LastModifiedAgent = colorVM._updateAgent;
            color.Code = colorVM.code;
            color.Name = colorVM.name;

            return color;
        }

    }
}
