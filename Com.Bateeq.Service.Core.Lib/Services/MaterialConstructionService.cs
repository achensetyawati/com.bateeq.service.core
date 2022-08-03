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
   public class MaterialConstructionService : BasicService<CoreDbContext, MaterialConstruction>, IMap<MaterialConstruction, MaterialConstructionViewModel>
    {
        public MaterialConstructionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public MaterialConstruction MapToModel(MaterialConstructionViewModel viewModel)
        {
            MaterialConstruction model = new MaterialConstruction();
            PropertyCopier<MaterialConstructionViewModel, MaterialConstruction>.Copy(viewModel, model);
            return model;
        }

        public MaterialConstructionViewModel MapToViewModel(MaterialConstruction model)
        {
            MaterialConstructionViewModel viewModel = new MaterialConstructionViewModel();
            PropertyCopier<MaterialConstruction, MaterialConstructionViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public override Tuple<List<MaterialConstruction>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<MaterialConstruction> Query = this.DbContext.MaterialConstructions;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Name","Remark", "_LastModifiedUtc"
            };

            Query = Query
                .Select(b => new MaterialConstruction
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    _LastModifiedUtc = b._LastModifiedUtc
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
            Pageable<MaterialConstruction> pageable = new Pageable<MaterialConstruction>(Query, Page - 1, Size);
            List<MaterialConstruction> Data = pageable.Data.ToList<MaterialConstruction>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }
        public override void OnCreating(MaterialConstruction model)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            do
            {
                model.Code = codeGenerator.GenerateCode();
            }
            while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

            base.OnCreating(model);
        }
    }
}
