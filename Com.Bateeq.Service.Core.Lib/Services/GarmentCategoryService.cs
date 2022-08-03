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
    public class GarmentCategoryService : BasicService<CoreDbContext, GarmentCategory>, IMap<GarmentCategory, GarmentCategoryViewModel>
    {
        public GarmentCategoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<GarmentCategory>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentCategory> Query = this.DbContext.GarmentCategories;

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name", "CodeRequirement"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Name", "CodeRequirement", "Uom"
            };

            Query = Query
                .Select(a => new GarmentCategory
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name=a.Name,
                    UomId=a.UomId,
                    UomUnit=a.UomUnit,
                    CodeRequirement=a.CodeRequirement
                });

            /* Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_LastModifiedUtc", General.DESCENDING);

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
            Pageable<GarmentCategory> pageable = new Pageable<GarmentCategory>(Query, Page - 1, Size);
            List<GarmentCategory> Data = pageable.Data.ToList<GarmentCategory>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public GarmentCategoryViewModel MapToViewModel(GarmentCategory category)
        {
            GarmentCategoryViewModel categoryViewModel = new GarmentCategoryViewModel();
            PropertyCopier<GarmentCategory, GarmentCategoryViewModel>.Copy(category, categoryViewModel);

            categoryViewModel.uom = new UomViewModel
            {
                Id = (int)category.UomId,
                Unit= category.UomUnit
            };
            categoryViewModel.name = category.Name;
            categoryViewModel.code = category.Code;
            categoryViewModel.codeRequirement = category.CodeRequirement;



            return categoryViewModel;
        }

        public GarmentCategory MapToModel(GarmentCategoryViewModel categoryVM)
        {
            GarmentCategory garmentCategory = new GarmentCategory();
            PropertyCopier<GarmentCategoryViewModel, GarmentCategory>.Copy(categoryVM, garmentCategory);

            if (!Equals(categoryVM.uom, null))
            {
                garmentCategory.UomId = categoryVM.uom.Id;
                garmentCategory.UomUnit = categoryVM.uom.Unit;
            }
            else
            {
                garmentCategory.UomId = null;
                garmentCategory.UomUnit = null;
            }
            garmentCategory.Name = categoryVM.name;
            garmentCategory.Code = categoryVM.code;
            garmentCategory.CodeRequirement = categoryVM.codeRequirement;

            return garmentCategory;
        }

        //public override void OnCreating(GarmentCategory model)
        //{
        //    CodeGenerator codeGenerator = new CodeGenerator();

        //    do
        //    {
        //        model.Code = codeGenerator.GenerateCode();
        //    }
        //    while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

        //    base.OnCreating(model);
        //}

    }
}
