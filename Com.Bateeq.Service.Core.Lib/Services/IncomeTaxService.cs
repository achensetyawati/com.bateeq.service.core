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
    public class IncomeTaxService : BasicService<CoreDbContext, IncomeTax>, IBasicUploadCsvService<IncomeTaxViewModel>, IMap<IncomeTax, IncomeTaxViewModel>
    {
        public IncomeTaxService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<IncomeTax>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<IncomeTax> Query = this.DbContext.IncomeTaxes;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "name", "rate","description"
            };

            Query = Query
                .Select(v => new IncomeTax
                {
                    Id = v.Id,
                    Name = v.Name,
                    Rate = v.Rate,
                    Description = v.Description,
                    COACodeCredit = v.COACodeCredit
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
            Pageable<IncomeTax> pageable = new Pageable<IncomeTax>(Query, Page - 1, Size);
            List<IncomeTax> Data = pageable.Data.ToList<IncomeTax>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public IncomeTaxViewModel MapToViewModel(IncomeTax incomeTax)
        {
            IncomeTaxViewModel incomeTaxVM = new IncomeTaxViewModel
            {
                Id = incomeTax.Id,
                UId = incomeTax.UId,
                _IsDeleted = incomeTax._IsDeleted,
                Active = incomeTax.Active,
                _CreatedUtc = incomeTax._CreatedUtc,
                _CreatedBy = incomeTax._CreatedBy,
                _CreatedAgent = incomeTax._CreatedAgent,
                _LastModifiedUtc = incomeTax._LastModifiedUtc,
                _LastModifiedBy = incomeTax._LastModifiedBy,
                _LastModifiedAgent = incomeTax._LastModifiedAgent,
                name = incomeTax.Name,
                rate = incomeTax.Rate,
                description = incomeTax.Description,
                COACodeCredit = incomeTax.COACodeCredit
            };

            return incomeTaxVM;
        }

        public IncomeTax MapToModel(IncomeTaxViewModel incomeTaxVM)
        {
            IncomeTax incomeTax = new IncomeTax
            {
                Id = incomeTaxVM.Id,
                UId = incomeTaxVM.UId,
                _IsDeleted = incomeTaxVM._IsDeleted,
                Active = incomeTaxVM.Active,
                _CreatedUtc = incomeTaxVM._CreatedUtc,
                _CreatedBy = incomeTaxVM._CreatedBy,
                _CreatedAgent = incomeTaxVM._CreatedAgent,
                _LastModifiedUtc = incomeTaxVM._LastModifiedUtc,
                _LastModifiedBy = incomeTaxVM._LastModifiedBy,
                _LastModifiedAgent = incomeTaxVM._LastModifiedAgent,
                Name = incomeTaxVM.name,
                Rate = !Equals(incomeTaxVM.rate, null) ? Convert.ToDouble(incomeTaxVM.rate) : null,
                Description = incomeTaxVM.description,
                COACodeCredit = incomeTaxVM.COACodeCredit
            };

            return incomeTax;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Nama", "Rate", "Deskripsi"
        };

        public List<string> CsvHeader => Header;

        public sealed class IncomeTaxMap : ClassMap<IncomeTaxViewModel>
        {
            public IncomeTaxMap()
            {
                Map(v => v.name).Index(0);
                Map(v => v.rate).Index(1).TypeConverter<StringConverter>();
                Map(v => v.description).Index(2);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<IncomeTaxViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (IncomeTaxViewModel incomeTaxVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(incomeTaxVM.name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                double Rate = 0;
                if (string.IsNullOrWhiteSpace(incomeTaxVM.rate))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kurs tidak boleh kosong, ");
                }
                else if (!double.TryParse(incomeTaxVM.rate, out Rate))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kurs harus numerik, ");
                }
                else if (Rate < 0 || Rate == 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kurs harus lebih besar dari 0, ");
                }
                else
                {
                    string[] RateSplit = incomeTaxVM.rate.Split('.');
                    if (RateSplit.Count().Equals(2) && RateSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kurs maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    
                    if (Data.Any(d => d != incomeTaxVM && d.name.Equals(incomeTaxVM.name) && d.rate.Equals(incomeTaxVM.rate)) || this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(incomeTaxVM.name) && d.Rate.Equals(Rate)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kombinasi Nama dan Rate tidak boleh sama, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    incomeTaxVM.rate = Rate;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Nama", incomeTaxVM.name);
                    Error.Add("Rate", incomeTaxVM.rate);
                    Error.Add("Deskripsi", incomeTaxVM.description);
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