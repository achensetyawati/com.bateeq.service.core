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
    public class ArticleMaterialCompositionService : BasicService<CoreDbContext, ArticleMaterialComposition>, IMap<ArticleMaterialComposition, ArticleMaterialCompositionViewModel>
    {
        public ArticleMaterialCompositionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



        public override Tuple<List<ArticleMaterialComposition>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleMaterialComposition> Query = this.DbContext.ArticleMaterialCompositions;
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
                .Select(b => new ArticleMaterialComposition
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
            Pageable<ArticleMaterialComposition> pageable = new Pageable<ArticleMaterialComposition>(Query, Page - 1, Size);
            List<ArticleMaterialComposition> Data = pageable.Data.ToList<ArticleMaterialComposition>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleMaterialCompositionViewModel MapToViewModel(ArticleMaterialComposition materialComposition)
        {
            ArticleMaterialCompositionViewModel materialCompositionVM = new ArticleMaterialCompositionViewModel();

            materialCompositionVM._id = materialComposition.Id;
            materialCompositionVM.UId = materialComposition.UId;
            materialCompositionVM._deleted = materialComposition._IsDeleted;
            materialCompositionVM._active = materialComposition.Active;
            materialCompositionVM._createdDate = materialComposition._CreatedUtc;
            materialCompositionVM._createdBy = materialComposition._CreatedBy;
            materialCompositionVM._createAgent = materialComposition._CreatedAgent;
            materialCompositionVM._updatedDate = materialComposition._LastModifiedUtc;
            materialCompositionVM._updatedBy = materialComposition._LastModifiedBy;
            materialCompositionVM._updateAgent = materialComposition._LastModifiedAgent;
            materialCompositionVM.code = materialComposition.Code;
            materialCompositionVM.name = materialComposition.Name;


            return materialCompositionVM;
        }

        public ArticleMaterialComposition MapToModel(ArticleMaterialCompositionViewModel materialCompositionVM)
        {
            ArticleMaterialComposition materialComposition = new ArticleMaterialComposition();

            materialComposition.Id = materialCompositionVM._id;
            materialComposition.UId = materialCompositionVM.UId;
            materialComposition._IsDeleted = materialCompositionVM._deleted;
            materialComposition.Active = materialCompositionVM._active;
            materialComposition._CreatedUtc = materialCompositionVM._createdDate;
            materialComposition._CreatedBy = materialCompositionVM._createdBy;
            materialComposition._CreatedAgent = materialCompositionVM._createAgent;
            materialComposition._LastModifiedUtc = materialCompositionVM._updatedDate;
            materialComposition._LastModifiedBy = materialCompositionVM._updatedBy;
            materialComposition._LastModifiedAgent = materialCompositionVM._updateAgent;
            materialComposition.Code = materialCompositionVM.code;
            materialComposition.Name = materialCompositionVM.name;

            return materialComposition;
        }
    }

   
}
