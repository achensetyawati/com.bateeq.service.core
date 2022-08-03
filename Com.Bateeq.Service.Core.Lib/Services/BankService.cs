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
    public class BankService : BasicService<CoreDbContext, Bank>, IMap<Bank, BankViewModel>
    {
        public BankService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Bank>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Bank> Query = this.DbContext.Banks;
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
                 "_id", "Code", "Description", "Name"
            };

            Query = Query
                .Select(b => new Bank
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Description = b.Description
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
            Pageable<Bank> pageable = new Pageable<Bank>(Query, Page - 1, Size);
            List<Bank> Data = pageable.Data.ToList<Bank>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public BankViewModel MapToViewModel(Bank bank)
        {
            BankViewModel bankViewModel = new BankViewModel();

            bankViewModel._id = bank.Id;
            bankViewModel.UId = bank.Uid;
            bankViewModel._deleted = bank._IsDeleted;
            bankViewModel._active = bank.Active;
            bankViewModel._createdDate = bank._CreatedUtc;
            bankViewModel._createdBy = bank._CreatedBy;
            bankViewModel._createAgent = bank._CreatedAgent;
            bankViewModel._updatedDate = bank._LastModifiedUtc;
            bankViewModel._updatedBy = bank._LastModifiedBy;
            bankViewModel._updateAgent = bank._LastModifiedAgent;
            bankViewModel.code = bank.Code;
            bankViewModel.description = bank.Description;
            bankViewModel.name = bank.Name;




            return bankViewModel;
        }

        public Bank MapToModel(BankViewModel bankViewModel)
        {
            Bank bank = new Bank();

            bank.Id = bankViewModel._id;
            bank.Uid = bankViewModel.UId;
            bank._IsDeleted = bankViewModel._deleted;
            bank.Active = bankViewModel._active;
            bank._CreatedUtc = bankViewModel._createdDate;
            bank._CreatedBy = bankViewModel._createdBy;
            bank._CreatedAgent = bankViewModel._createAgent;
            bank._LastModifiedUtc = bankViewModel._updatedDate;
            bank._LastModifiedBy = bankViewModel._updatedBy;
            bank._LastModifiedAgent = bankViewModel._updateAgent;
            bank.Code = bankViewModel.code;
            bank.Description = bankViewModel.description;
            bank.Name = bankViewModel.name;

            return bank;
        }
    }
}
