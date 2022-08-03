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
    public class StandardTestsService : BasicService<CoreDbContext, StandardTests>, IMap<StandardTests, StandardTestsViewModel>
    {
        public StandardTestsService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public StandardTests MapToModel(StandardTestsViewModel viewModel)
        {
            StandardTests model = new StandardTests();
            PropertyCopier<StandardTestsViewModel, StandardTests>.Copy(viewModel, model);
            return model;
        }

        public StandardTestsViewModel MapToViewModel(StandardTests model)
        {
            StandardTestsViewModel viewModel = new StandardTestsViewModel();
            PropertyCopier<StandardTests, StandardTestsViewModel>.Copy(model, viewModel);
            return viewModel;
        }

        public override Tuple<List<StandardTests>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<StandardTests> Query = this.DbContext.StandardTests;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Name","Code"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Name", "Remark", "_LastModifiedUtc"
            };

            Query = Query
                .Select(b => new StandardTests
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Remark = b.Remark,
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
            Pageable<StandardTests> pageable = new Pageable<StandardTests>(Query, Page - 1, Size);
            List<StandardTests> Data = pageable.Data.ToList<StandardTests>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public override void OnCreating(StandardTests model)
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
