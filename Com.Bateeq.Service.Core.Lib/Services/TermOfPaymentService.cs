using Com.DanLiris.Service.Core.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib.Interfaces;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class TermOfPaymentService : BasicService<CoreDbContext, TermOfPayment>, IMap<TermOfPayment, TermOfPaymentViewModel>
    {
        public TermOfPaymentService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<TermOfPayment>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<TermOfPayment> Query = this.DbContext.TermOfPayments;
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
                "Id", "Code", "Name", "isExport"
            };

            Query = Query
                .Select(t => new TermOfPayment
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name = t.Name,
                    IsExport = t.IsExport
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
            Pageable<TermOfPayment> pageable = new Pageable<TermOfPayment>(Query, Page - 1, Size);
            List<TermOfPayment> Data = pageable.Data.ToList<TermOfPayment>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public TermOfPaymentViewModel MapToViewModel(TermOfPayment termOfPayment)
        {
            TermOfPaymentViewModel termOfPaymentVM = new TermOfPaymentViewModel();
            PropertyCopier<TermOfPayment, TermOfPaymentViewModel>.Copy(termOfPayment, termOfPaymentVM);
            return termOfPaymentVM;
        }

        public TermOfPayment MapToModel(TermOfPaymentViewModel termOfPaymentVM)
        {
            TermOfPayment termOfPayment = new TermOfPayment();
            PropertyCopier<TermOfPaymentViewModel, TermOfPayment>.Copy(termOfPaymentVM, termOfPayment);
            return termOfPayment;
        }

        public override void OnCreating(TermOfPayment model)
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