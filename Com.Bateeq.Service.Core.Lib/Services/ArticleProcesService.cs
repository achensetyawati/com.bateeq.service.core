using Com.DanLiris.Service.Core.Lib.Helpers;
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
using Com.DanLiris.Service.Core.Lib.Interfaces;


namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ArticleProcesService : BasicService<CoreDbContext, ArticleProces>, IMap<ArticleProces, ArticleProcesViewModel>
    {

        public ArticleProcesService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<ArticleProces>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleProces> Query = this.DbContext.ArticleProcess;
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
                 "_id","Code", "Name", "Description"
            };

            Query = Query
                .Select(b => new ArticleProces
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
            Pageable<ArticleProces> pageable = new Pageable<ArticleProces>(Query, Page - 1, Size);
            List<ArticleProces> Data = pageable.Data.ToList<ArticleProces>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleProcesViewModel MapToViewModel(ArticleProces proces)
        {
            ArticleProcesViewModel procesVM = new ArticleProcesViewModel();

            procesVM._id = proces.Id;
            procesVM.UId = proces.UId;
            procesVM._deleted = proces._IsDeleted;
            procesVM._active = proces.Active;
            procesVM._createdDate = proces._CreatedUtc;
            procesVM._createdBy = proces._CreatedBy;
            procesVM._createAgent = proces._CreatedAgent;
            procesVM._updatedDate = proces._LastModifiedUtc;
            procesVM._updatedBy = proces._LastModifiedBy;
            procesVM._updateAgent = proces._LastModifiedAgent;
            procesVM.code = proces.Code;
            procesVM.name = proces.Name;


            return procesVM;
        }

        public ArticleProces MapToModel(ArticleProcesViewModel procesVM)
        {
            ArticleProces proces = new ArticleProces();

            proces.Id = procesVM._id;
            proces.UId = procesVM.UId;
            proces._IsDeleted = procesVM._deleted;
            proces.Active = procesVM._active;
            proces._CreatedUtc = procesVM._createdDate;
            proces._CreatedBy = procesVM._createdBy;
            proces._CreatedAgent = procesVM._createAgent;
            proces._LastModifiedUtc = procesVM._updatedDate;
            proces._LastModifiedBy = procesVM._updatedBy;
            proces._LastModifiedAgent = procesVM._updateAgent;
            proces.Code = procesVM.code;
            proces.Name = procesVM.name;

            return proces;
        }



        //public sealed class MaterialMap : ClassMap<ArticleProcesViewModel>
        //{
        //    public MaterialMap()
        //    {
        //        Map(c => c.code).Index(0);
        //        Map(c => c.name).Index(1);
        //    }
        //}
    }
}
