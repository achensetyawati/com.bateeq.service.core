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
using CsvHelper.Configuration;
using System.Dynamic;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class GarmentCurrencyService : BasicService<CoreDbContext, GarmentCurrency>, IBasicUploadCsvService<GarmentCurrencyViewModel>, IMap<GarmentCurrency, GarmentCurrencyViewModel>
    {
        public GarmentCurrencyService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<GarmentCurrency>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentCurrency> Query = this.DbContext.GarmentCurrencies;
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "code", "rate", "date"
            };

            Query = Query
                .Select(g => new GarmentCurrency
                {
                    Id = g.Id,
                    Code = g.Code,
                    Rate = g.Rate,
                    Date = g.Date
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
            Pageable<GarmentCurrency> pageable = new Pageable<GarmentCurrency>(Query, Page - 1, Size);
            List<GarmentCurrency> Data = pageable.Data.ToList<GarmentCurrency>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public GarmentCurrencyViewModel MapToViewModel(GarmentCurrency garmentCurrency)
        {
            GarmentCurrencyViewModel garmentCurrencyVM = new GarmentCurrencyViewModel();

            garmentCurrencyVM.Id = garmentCurrency.Id;
            garmentCurrencyVM.UId = garmentCurrency.UId;
            garmentCurrencyVM._IsDeleted = garmentCurrency._IsDeleted;
            garmentCurrencyVM.Active = garmentCurrency.Active;
            garmentCurrencyVM._CreatedUtc = garmentCurrency._CreatedUtc;
            garmentCurrencyVM._CreatedBy = garmentCurrency._CreatedBy;
            garmentCurrencyVM._CreatedAgent = garmentCurrency._CreatedAgent;
            garmentCurrencyVM._LastModifiedUtc = garmentCurrency._LastModifiedUtc;
            garmentCurrencyVM._LastModifiedBy = garmentCurrency._LastModifiedBy;
            garmentCurrencyVM._LastModifiedAgent = garmentCurrency._LastModifiedAgent;
            garmentCurrencyVM.code = garmentCurrency.Code;
            garmentCurrencyVM.date = garmentCurrency.Date.ToLocalTime();
            garmentCurrencyVM.rate = garmentCurrency.Rate;

            return garmentCurrencyVM;
        }

        public GarmentCurrency MapToModel(GarmentCurrencyViewModel garmentCurrencyVM)
        {
            GarmentCurrency garmentCurrency = new GarmentCurrency();

            garmentCurrency.Id = garmentCurrencyVM.Id;
            garmentCurrency.UId = garmentCurrencyVM.UId;
            garmentCurrency._IsDeleted = garmentCurrencyVM._IsDeleted;
            garmentCurrency.Active = garmentCurrencyVM.Active;
            garmentCurrency._CreatedUtc = garmentCurrencyVM._CreatedUtc;
            garmentCurrency._CreatedBy = garmentCurrencyVM._CreatedBy;
            garmentCurrency._CreatedAgent = garmentCurrencyVM._CreatedAgent;
            garmentCurrency._LastModifiedUtc = garmentCurrencyVM._LastModifiedUtc;
            garmentCurrency._LastModifiedBy = garmentCurrencyVM._LastModifiedBy;
            garmentCurrency._LastModifiedAgent = garmentCurrencyVM._LastModifiedAgent;
            garmentCurrency.Code = garmentCurrencyVM.code;
            garmentCurrency.Date = garmentCurrencyVM.date;
            garmentCurrency.Rate = !Equals(garmentCurrencyVM.rate, null) ? Convert.ToDouble(garmentCurrencyVM.rate) : null; /* Check Null */

            return garmentCurrency;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Mata Uang", "Kurs"
        };

        public List<string> CsvHeader => Header;

        public sealed class GarmentCurrencyMap : ClassMap<GarmentCurrencyViewModel>
        {
            public GarmentCurrencyMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.rate).Index(1).TypeConverter<StringConverter>();
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<GarmentCurrencyViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            const string DATE_KEYWORD = "date";

            DateTime Date;
            string DateString = Body.SingleOrDefault(s => s.Key.Equals(DATE_KEYWORD)).Value;
            bool ValidDate = DateTime.TryParseExact(DateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);

            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (GarmentCurrencyViewModel garmentCurrencyVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(garmentCurrencyVM.code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak boleh kosong, ");
                }

                double Rate = 0;
                if (string.IsNullOrWhiteSpace(garmentCurrencyVM.rate))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kurs tidak boleh kosong, ");
                }
                else if (!double.TryParse(garmentCurrencyVM.rate, out Rate))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kurs harus numerik, ");
                }
                else if (Rate < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kurs harus lebih besar dari 0, ");
                }
                else
                {
                    string[] RateSplit = garmentCurrencyVM.rate.Split('.');
                    if (RateSplit.Count().Equals(2) && RateSplit[1].Length > 4)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kurs maksimal memiliki 4 digit dibelakang koma, ");
                    }
                }

                if (!ValidDate)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Tanggal tidak boleh kosong, ");
                }
                else if (Date > DateTime.Now)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Tanggal tidak boleh lebih dari tanggal hari ini, ");
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if (!this.DbContext.Set<Currency>().Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(garmentCurrencyVM.code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak terdaftar dalam master Mata Uang, ");
                    }

                    if (Data.Any(d => d != garmentCurrencyVM && d.code.Equals(garmentCurrencyVM.code) && d.date.Equals(garmentCurrencyVM.date)) || this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(garmentCurrencyVM.code) && d.Date.Equals(Date)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Mata Uang dan Tanggal tidak boleh duplikat, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    garmentCurrencyVM.rate = Rate;
                    garmentCurrencyVM.date = Date.ToUniversalTime();
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Mata Uang", garmentCurrencyVM.code);
                    Error.Add("Kurs", garmentCurrencyVM.rate);
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

        public List<GarmentCurrency> GetByIds(List<int> ids)
        {
            return this.DbSet.Where(p => ids.Contains(p.Id) && p._IsDeleted == false)
                .ToList();
        }

        public List<GarmentCurrency> GetByCode(string code)
        {
            return this.DbSet.Where(p => code.Contains(p.Code) && p._IsDeleted == false)
                .ToList();
        }

        public GarmentCurrency GetSingleByCode(string code)
        {
            return DbSet.OrderByDescending(o => o._LastModifiedUtc).FirstOrDefault(f => f.Code == code);
        }

        public GarmentCurrency GetSingleByCodeDate(string code, DateTimeOffset date)
        {
            var currencyWithCodeYear = DbSet.OrderBy(o => o.Date).Where(f => f.Code == code && f.Date.Year == date.Year);
            if (currencyWithCodeYear.Count() == 0)
            {
                return GetSingleByCode(code);
            }
            else
            {
                return date >= currencyWithCodeYear.Last().Date ? currencyWithCodeYear.Last() :
                    date <= currencyWithCodeYear.First().Date ? currencyWithCodeYear.First() :
                    currencyWithCodeYear.Last(d => d.Date <= date);
                
            }
        }
    }
}