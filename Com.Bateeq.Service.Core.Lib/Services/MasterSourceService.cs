using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class MasterSourceService : BasicService<CoreDbContext, MasterSource>, IMap<MasterSource, MasterSourceViewModel>
    {
        public MasterSourceService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public MasterSource MapToModel(MasterSourceViewModel viewModel)
        {
            MasterSource masterSource = new MasterSource();

            masterSource.Id = viewModel.Id;
            masterSource._IsDeleted = viewModel._IsDeleted;
            masterSource.Active = viewModel.Active;
            masterSource._CreatedUtc = viewModel._CreatedUtc;
            masterSource._CreatedBy = viewModel._CreatedBy;
            masterSource._CreatedAgent = viewModel._CreatedAgent;
            masterSource._LastModifiedUtc = viewModel._LastModifiedUtc;
            masterSource._LastModifiedBy = viewModel._LastModifiedBy;
            masterSource._LastModifiedAgent = viewModel._LastModifiedAgent;
            masterSource.Code = viewModel.Code;
            masterSource.Name = viewModel.Name;
            masterSource.Description = viewModel.Description;

            return masterSource;
        }

        public MasterSourceViewModel MapToViewModel(MasterSource model)
        {
            MasterSourceViewModel viewModel = new MasterSourceViewModel();

            viewModel.Id = model.Id;
            viewModel._IsDeleted = model._IsDeleted;
            viewModel.Active = model.Active;
            viewModel._CreatedUtc = model._CreatedUtc;
            viewModel._CreatedBy = model._CreatedBy;
            viewModel._CreatedAgent = model._CreatedAgent;
            viewModel._LastModifiedUtc = model._LastModifiedUtc;
            viewModel._LastModifiedBy = model._LastModifiedBy;
            viewModel._LastModifiedAgent = model._LastModifiedAgent;
            viewModel.Code = model.Code;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;

            return viewModel;
        }

        public override Tuple<List<MasterSource>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MasterSource> Query = this.DbContext.MasterSources;
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
                 "Id", "Code", "Name", "Description"
            };

            Query = Query
                .Select(b => new MasterSource
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
            Pageable<MasterSource> pageable = new Pageable<MasterSource>(Query, Page - 1, Size);
            List<MasterSource> Data = pageable.Data.ToList<MasterSource>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public override void OnCreating(MasterSource model)
        {
            base.OnCreating(model);
        }
    }
}
