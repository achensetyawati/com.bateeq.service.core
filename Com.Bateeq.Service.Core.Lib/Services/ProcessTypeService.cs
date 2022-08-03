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
    public class ProcessTypeService : BasicService<CoreDbContext, ProcessType>, IMap<ProcessType, ProcessTypeViewModel>
    {
        public ProcessTypeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public ProcessType MapToModel(ProcessTypeViewModel viewModel)
        {
            ProcessType model = new ProcessType();
            PropertyCopier<ProcessTypeViewModel, ProcessType>.Copy(viewModel, model);
            model.OrderTypeId = viewModel.OrderType.Id;
            model.OrderTypeCode = viewModel.OrderType.Code;
            model.OrderTypeName = viewModel.OrderType.Name;
            model.OrderTypeRemark = viewModel.OrderType.Remark;
            return model;
        }

        public ProcessTypeViewModel MapToViewModel(ProcessType model)
        {
            ProcessTypeViewModel viewModel = new ProcessTypeViewModel();
            PropertyCopier<ProcessType, ProcessTypeViewModel>.Copy(model, viewModel);
            viewModel.OrderType = new OrderTypeViewModel();
            viewModel.OrderType.Id = model.OrderTypeId;
            viewModel.OrderType.Code = model.OrderTypeCode;
            viewModel.OrderType.Name = model.OrderTypeName;
            viewModel.OrderType.Remark = model.OrderTypeRemark;

            return viewModel;
        }

        public override Tuple<List<ProcessType>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ProcessType> Query = this.DbContext.ProcessType;
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
                "Id", "Code", "Name", "Remark", "_LastModifiedUtc", "OrderType"
            };

            Query = Query
                .Select(b => new ProcessType
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Remark = b.Remark,
                    OrderTypeCode = b.OrderTypeCode,
                    OrderTypeId = b.OrderTypeId,
                    OrderTypeName = b.OrderTypeName,
                    OrderTypeRemark = b.OrderTypeRemark,
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
            Pageable<ProcessType> pageable = new Pageable<ProcessType>(Query, Page - 1, Size);
            List<ProcessType> Data = pageable.Data.ToList<ProcessType>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public override void OnCreating(ProcessType model)
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
