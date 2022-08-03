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
    public class BudgetService : BasicService<CoreDbContext, Budget>, IBasicUploadCsvService<BudgetViewModel>, IMap<Budget, BudgetViewModel>
    {
        public BudgetService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Budget>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<Budget> Query = this.DbContext.Budgets;

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
                "_id", "code", "name"
            };

            Query = Query
                .Select(b => new Budget
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
            Pageable<Budget> pageable = new Pageable<Budget>(Query, Page - 1, Size);
            List<Budget> Data = pageable.Data.ToList<Budget>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public BudgetViewModel MapToViewModel(Budget budget)
        {
            BudgetViewModel budgetVM = new BudgetViewModel();

            budgetVM._id = budget.Id;
            budgetVM.UId = budget.UId;
            budgetVM._deleted = budget._IsDeleted;
            budgetVM._active = budget.Active;
            budgetVM._createdDate = budget._CreatedUtc;
            budgetVM._createdBy = budget._CreatedBy;
            budgetVM._createAgent = budget._CreatedAgent;
            budgetVM._updatedDate = budget._LastModifiedUtc;
            budgetVM._updatedBy = budget._LastModifiedBy;
            budgetVM._updateAgent = budget._LastModifiedAgent;
            budgetVM.code = budget.Code;
            budgetVM.name = budget.Name;

            return budgetVM;
        }

        public Budget MapToModel(BudgetViewModel budgetVM)
        {
            Budget budget = new Budget();

            budget.Id = budgetVM._id;
            budget.UId = budgetVM.UId;
            budget._IsDeleted = budgetVM._deleted;
            budget.Active = budgetVM._active;
            budget._CreatedUtc = budgetVM._createdDate;
            budget._CreatedBy = budgetVM._createdBy;
            budget._CreatedAgent = budgetVM._createAgent;
            budget._LastModifiedUtc = budgetVM._updatedDate;
            budget._LastModifiedBy = budgetVM._updatedBy;
            budget._LastModifiedAgent = budgetVM._updateAgent;
            budget.Code = budgetVM.code;
            budget.Name = budgetVM.name;

            return budget;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Nama"
        };

        public List<string> CsvHeader => Header;

        public sealed class BudgetMap : ClassMap<BudgetViewModel>
        {
            public BudgetMap()
            {
                Map(b => b.code).Index(0);
                Map(b => b.name).Index(1);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<BudgetViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (BudgetViewModel budgetVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(budgetVM.code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if(Data.Any(d => d != budgetVM && d.code.Equals(budgetVM.code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(budgetVM.name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if(Data.Any(d => d != budgetVM && d.name.Equals(budgetVM.name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if(string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(budgetVM.code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(budgetVM.name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMessage)) /* Not Empty */
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode", budgetVM.code);
                    Error.Add("Nama", budgetVM.name);
                    Error.Add("Error", ErrorMessage);

                    ErrorList.Add(Error);
                }
            }

            if (ErrorList.Count > 0)
            {
                Valid = false;
            }

            return Tuple.Create(Valid, ErrorList);
        }
    }
}