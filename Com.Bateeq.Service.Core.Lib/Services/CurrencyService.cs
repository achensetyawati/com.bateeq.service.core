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
using Com.DanLiris.Service.Core.Lib.Interfaces;
using CsvHelper.Configuration;
using System.Dynamic;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class CurrencyService : BasicService<CoreDbContext, Currency>, IBasicUploadCsvService<CurrencyViewModel>, IMap<Currency, CurrencyViewModel>
    {
        public CurrencyService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Currency>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Currency> Query = this.DbContext.Currencies;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Symbol"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Symbol", "Description", "Rate"
            };

            Query = Query
                .Select(c => new Currency
                {
                    Id = c.Id,
                    Code = c.Code,
                    Symbol = c.Symbol,
                    Description = c.Description,
                    Rate=c.Rate,
                    _LastModifiedUtc = c._LastModifiedUtc
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
            Pageable<Currency> pageable = new Pageable<Currency>(Query, Page - 1, Size);
            List<Currency> Data = pageable.Data.ToList<Currency>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public CurrencyViewModel MapToViewModel(Currency currency)
        {
            CurrencyViewModel currencyVM = new CurrencyViewModel();

            currencyVM.Id = currency.Id;
            currencyVM.UId = currency.UId;
            currencyVM._IsDeleted = currency._IsDeleted;
            currencyVM.Active = currency.Active;
            currencyVM._CreatedUtc = currency._CreatedUtc;
            currencyVM._CreatedBy = currency._CreatedBy;
            currencyVM._CreatedAgent = currency._CreatedAgent;
            currencyVM._LastModifiedUtc = currency._LastModifiedUtc;
            currencyVM._LastModifiedBy = currency._LastModifiedBy;
            currencyVM._LastModifiedAgent = currency._LastModifiedAgent;
            currencyVM.Code = currency.Code;
            currencyVM.Symbol = currency.Symbol;
            currencyVM.Rate = currency.Rate;
            currencyVM.Description = currency.Description;

            return currencyVM;
        }

        public Currency MapToModel(CurrencyViewModel currencyVM)
        {
            Currency currency = new Currency();

            currency.Id = currencyVM.Id;
            currency.UId = currencyVM.UId;
            currency._IsDeleted = currencyVM._IsDeleted;
            currency.Active = currencyVM.Active;
            currency._CreatedUtc = currencyVM._CreatedUtc;
            currency._CreatedBy = currencyVM._CreatedBy;
            currency._CreatedAgent = currencyVM._CreatedAgent;
            currency._LastModifiedUtc = currencyVM._LastModifiedUtc;
            currency._LastModifiedBy = currencyVM._LastModifiedBy;
            currency._LastModifiedAgent = currencyVM._LastModifiedAgent;
            currency.Code = currencyVM.Code;
            currency.Symbol = currencyVM.Symbol;
            currency.Rate = !Equals(currencyVM.Rate, null) ? Convert.ToDouble(currencyVM.Rate) : null; /* Check Null */
            currency.Description = currencyVM.Description;

            return currency;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Simbol", "Rate", "Keterangan"
        };

        public List<string> CsvHeader => Header;

        public sealed class CurrencyMap : ClassMap<CurrencyViewModel>
        {
            public CurrencyMap()
            {
                Map(c => c.Code).Index(0);
                Map(c => c.Symbol).Index(1);
                Map(c => c.Rate).Index(2).TypeConverter<StringConverter>();
                Map(c => c.Description).Index(3);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<CurrencyViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (CurrencyViewModel currencyVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(currencyVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != currencyVM && d.Code.Equals(currencyVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if(string.IsNullOrWhiteSpace(currencyVM.Symbol))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Simbol tidak boleh kosong, ");
                }

                double Rate = 0;
                if (string.IsNullOrWhiteSpace(currencyVM.Rate))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Rate tidak boleh kosong, ");
                }
                else if (!double.TryParse(currencyVM.Rate, out Rate))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Rate harus numerik, ");
                }
                else if (Rate < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Rate harus lebih besar dari 0, ");
                }
                else
                {
                    string[] RateSplit = currencyVM.Rate.Split('.');
                    if (RateSplit.Count().Equals(2) && RateSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Rate maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }

                if (string.IsNullOrWhiteSpace(currencyVM.Description))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Keterangan tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != currencyVM && d.Description.Equals(currencyVM.Description)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Keterangan tidak boleh duplikat, ");
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if(this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(currencyVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if(this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Description.Equals(currencyVM.Description)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Keterangan tidak boleh duplikat, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    currencyVM.Rate = Rate;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode", currencyVM.Code);
                    Error.Add("Simbol", currencyVM.Symbol);
                    Error.Add("Rate", currencyVM.Rate);
                    Error.Add("Keterangan", currencyVM.Description);
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