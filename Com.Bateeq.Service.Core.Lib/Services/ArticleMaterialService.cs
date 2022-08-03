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
    public class ArticleMaterialService : BasicService<CoreDbContext, ArticleMaterial>, IMap<ArticleMaterial, ArticleMaterialViewModel>
    {
        public ArticleMaterialService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



        public override Tuple<List<ArticleMaterial>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ArticleMaterial> Query = this.DbContext.ArticleMaterials;
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
                .Select(b => new ArticleMaterial
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
            Pageable<ArticleMaterial> pageable = new Pageable<ArticleMaterial>(Query, Page - 1, Size);
            List<ArticleMaterial> Data = pageable.Data.ToList<ArticleMaterial>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ArticleMaterialViewModel MapToViewModel(ArticleMaterial material)
        {
            ArticleMaterialViewModel materialVM = new ArticleMaterialViewModel();

            materialVM._id = material.Id;
            materialVM.UId = material.UId;
            materialVM._deleted = material._IsDeleted;
            materialVM._active = material.Active;
            materialVM._createdDate = material._CreatedUtc;
            materialVM._createdBy = material._CreatedBy;
            materialVM._createAgent = material._CreatedAgent;
            materialVM._updatedDate = material._LastModifiedUtc;
            materialVM._updatedBy = material._LastModifiedBy;
            materialVM._updateAgent = material._LastModifiedAgent;
            materialVM.code = material.Code;
            materialVM.name = material.Name;


            return materialVM;
        }

        public ArticleMaterial MapToModel(ArticleMaterialViewModel materialVM)
        {
            ArticleMaterial material = new ArticleMaterial();

            material.Id = materialVM._id;
            material.UId = materialVM.UId;
            material._IsDeleted = materialVM._deleted;
            material.Active = materialVM._active;
            material._CreatedUtc = materialVM._createdDate;
            material._CreatedBy = materialVM._createdBy;
            material._CreatedAgent = materialVM._createAgent;
            material._LastModifiedUtc = materialVM._updatedDate;
            material._LastModifiedBy = materialVM._updatedBy;
            material._LastModifiedAgent = materialVM._updateAgent;
            material.Code = materialVM.code;
            material.Name = materialVM.name;

            return material;
        }



        public sealed class MaterialMap : ClassMap<ArticleMaterialViewModel>
        {
            public MaterialMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
            }
        }



    }
}
