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
    public class ArticleCategoryService : BasicService<CoreDbContext, ArticleCategory>, IBasicUploadCsvService<ArticleCategoryViewModel>, IMap<ArticleCategory, ArticleCategoryViewModel>
    {
        public ArticleCategoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<ArticleCategory>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleCategory> Query = this.DbContext.ArticleCategories;
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
                .Select(b => new ArticleCategory
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
            Pageable<ArticleCategory> pageable = new Pageable<ArticleCategory>(Query, Page - 1, Size);
            List<ArticleCategory> Data = pageable.Data.ToList<ArticleCategory>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        //public  Tuple<List<CategoryViewModel>, int, Dictionary<string, string>> JoinDivision(int Page = 1, int Size = 25, string Order = "{}", string Keyword = "", string Filter = "{}")
        //{
        //    //IQueryable<Category> Query = this.DbContext.Categories;
        //    //IQueryable<Division> divisions = DbContext.Divisions;

        //    var Query = from t1 in DbContext.Categories
        //             from t2 in DbContext.Divisions
        //             where ((!string.IsNullOrEmpty(t1.Name) && t1.Name.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
        //                    || (!string.IsNullOrEmpty(t2.Name) && t2.Name.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0))
        //             select new CategoryViewModel()
        //             {
        //                 code = t1.Code,
        //                 codeRequirement = t1.CodeRequirement,
        //                 divisionId = t2.Id,
        //                 divisionName = t2.Name,
        //                 name = t1.Name,
        //                 UId = t1.UId,
        //                 _id = t1.Id,
        //                 _updatedDate = t1._LastModifiedUtc
        //             };
        //    Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
        //    if (FilterDictionary != null && !FilterDictionary.Count.Equals(0))
        //    {
        //        foreach (var f in FilterDictionary)
        //        {
        //            string Key = f.Key;
        //            object Value = f.Value;
        //            string filterQuery = string.Concat(string.Empty, Key, " == @0");

        //            Query = Query.Where(filterQuery, Value);
        //        }
        //    }
        //    //qu = ConfigureFilter(Query, FilterDictionary);
        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

        //    ///* Search With Keyword */
        //    //if (Keyword != null)
        //    //{
        //    //    List<string> SearchAttributes = new List<string>()
        //    //    {
        //    //        "Code", "Name"
        //    //    };

        //    //    Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
        //    //}

        //    ///* Const Select */
        //    //List<string> SelectedFields = new List<string>()
        //    //{
        //    //    "_id", "code", "name"
        //    //};

        //    //Query = Query
        //    //    .Select(b => new Category
        //    //    {
        //    //        Id = b.Id,
        //    //        Code = b.Code,
        //    //        Name = b.Name
        //    //    });

        //    /* Order */
        //    if (OrderDictionary.Count.Equals(0))
        //    {
        //        OrderDictionary.Add("_updatedDate", General.DESCENDING);

        //        Query = Query.OrderByDescending(b => b._updatedDate); /* Default Order */
        //    }
        //    else
        //    {
        //        string Key = OrderDictionary.Keys.First();
        //        string OrderType = OrderDictionary[Key];
        //        string TransformKey = General.TransformOrderBy(Key);

        //        BindingFlags IgnoreCase = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

        //        Query = OrderType.Equals(General.ASCENDING) ?
        //            Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
        //            Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
        //    }

        //    /* Pagination */
        //    Pageable<CategoryViewModel> pageable = new Pageable<CategoryViewModel>(Query, Page - 1, Size);
        //    List<CategoryViewModel> Data = pageable.Data.ToList<CategoryViewModel>();

        //    int TotalData = pageable.TotalCount;

        //    return Tuple.Create(Data, TotalData, OrderDictionary);
        //}

        public ArticleCategoryViewModel MapToViewModel(ArticleCategory category)
        {
            ArticleCategoryViewModel categoryVM = new ArticleCategoryViewModel();

            categoryVM._id = category.Id;
            categoryVM.UId = category.UId;
            categoryVM._deleted = category._IsDeleted;
            categoryVM._active = category.Active;
            categoryVM._createdDate = category._CreatedUtc;
            categoryVM._createdBy = category._CreatedBy;
            categoryVM._createAgent = category._CreatedAgent;
            categoryVM._updatedDate = category._LastModifiedUtc;
            categoryVM._updatedBy = category._LastModifiedBy;
            categoryVM._updateAgent = category._LastModifiedAgent;
            categoryVM.code = category.Code;
            categoryVM.name = category.Name;
      

            return categoryVM;
        }

        public ArticleCategory MapToModel(ArticleCategoryViewModel categoryVM)
        {
            ArticleCategory category = new ArticleCategory();

            category.Id = categoryVM._id;
            category.UId = categoryVM.UId;
            category._IsDeleted = categoryVM._deleted;
            category.Active = categoryVM._active;
            category._CreatedUtc = categoryVM._createdDate;
            category._CreatedBy = categoryVM._createdBy;
            category._CreatedAgent = categoryVM._createAgent;
            category._LastModifiedUtc = categoryVM._updatedDate;
            category._LastModifiedBy = categoryVM._updatedBy;
            category._LastModifiedAgent = categoryVM._updateAgent;
            category.Code = categoryVM.code;
            category.Name = categoryVM.name;

            return category;
        }

        public Tuple<bool, List<object>> UploadValidate(List<ArticleCategoryViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            throw new NotImplementedException();
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Nama", "Kode Kebutuhan"
        };

        public List<string> CsvHeader => Header;

        public sealed class CategoryMap : ClassMap<ArticleCategoryViewModel>
        {
            public CategoryMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
            }
        }

       
    }
}