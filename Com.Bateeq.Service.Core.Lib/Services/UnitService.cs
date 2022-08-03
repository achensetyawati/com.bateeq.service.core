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
using CsvHelper.Configuration;
using System.Dynamic;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class UnitService : BasicService<CoreDbContext, Unit>, IBasicUploadCsvService<UnitViewModel>, IMap<Unit, UnitViewModel>
    {
        private readonly string[] Types = { "Lokal", "Ekspor", "Internal" };
        private readonly string[] Countries = { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua and Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Canada", "Cape Verde", "Cayman Islands", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cruise Ship", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Kyrgyz Republic", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "Norway", "Oman", "Pakistan", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre and Miquelon", "Samoa", "San Marino", "Satellite", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sri Lanka", "St Kitts and Nevis", "St Lucia", "St Vincent", "St. Lucia", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks and Caicos", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States of America", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe" };

        public UnitService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Unit>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<Unit> Query = this.DbContext.Units;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "DivisionName", "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Division", "Name", "COACode"
            };

            Query = Query
                .Select(u => new Unit
                {
                    Id = u.Id,
                    Code = u.Code,
                    DivisionId = u.DivisionId,
                    DivisionCode = u.DivisionCode,
                    DivisionName = u.DivisionName,
                    Name = u.Name,
                    COACode = u.COACode
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
            Pageable<Unit> pageable = new Pageable<Unit>(Query, Page - 1, Size);
            List<Unit> Data = pageable.Data.ToList<Unit>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public UnitViewModel MapToViewModel(Unit unit)
        {
            UnitViewModel unitVM = new UnitViewModel();
            unitVM.Division = new DivisionViewModel();

            unitVM.Id = unit.Id;
            unitVM.UId = unit.UId;
            unitVM._IsDeleted = unit._IsDeleted;
            unitVM.Active = unit.Active;
            unitVM._CreatedUtc = unit._CreatedUtc;
            unitVM._CreatedBy = unit._CreatedBy;
            unitVM._CreatedAgent = unit._CreatedAgent;
            unitVM._LastModifiedUtc = unit._LastModifiedUtc;
            unitVM._LastModifiedBy = unit._LastModifiedBy;
            unitVM._LastModifiedAgent = unit._LastModifiedAgent;
            unitVM.Code = unit.Code;
            unitVM.Division.Id = unit.DivisionId;
            unitVM.Division.Code = unit.DivisionCode;
            unitVM.Division.Name = unit.DivisionName;
            unitVM.Name = unit.Name;
            unitVM.Description = unit.Description;
            unitVM.COACode = unit.COACode;
            
            return unitVM;
        }

        public Unit MapToModel(UnitViewModel unitVM)
        {
            Unit unit = new Unit();

            unit.Id = unitVM.Id;
            unit.UId = unitVM.UId;
            unit._IsDeleted = unitVM._IsDeleted;
            unit.Active = unitVM.Active;
            unit._CreatedUtc = unitVM._CreatedUtc;
            unit._CreatedBy = unitVM._CreatedBy;
            unit._CreatedAgent = unitVM._CreatedAgent;
            unit._LastModifiedUtc = unitVM._LastModifiedUtc;
            unit._LastModifiedBy = unitVM._LastModifiedBy;
            unit._LastModifiedAgent = unitVM._LastModifiedAgent;
            unit.Code = unitVM.Code;
            unit.DivisionId = unitVM.Division.Id;
            unit.DivisionCode = unitVM.Division.Code;
            unit.DivisionName = unitVM.Division.Name;
            unit.Name = unitVM.Name;
            unit.Description = unitVM.Description;
            unit.COACode = unitVM.COACode;

            return unit;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode Unit", "Divisi", "Nama", "Deskripsi"
        };

        public List<string> CsvHeader => Header;

        public sealed class UnitMap : ClassMap<UnitViewModel>
        {
            public UnitMap()
            {
                Map(b => b.Code).Index(0);
                Map(b => b.Division.Name).Index(1);
                Map(b => b.Name).Index(2);
                Map(b => b.Description).Index(3);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<UnitViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            Division division = null;

            foreach (UnitViewModel unitVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(unitVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != unitVM && d.Code.Equals(unitVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(unitVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != unitVM && d.Name.Equals(unitVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if(string.IsNullOrWhiteSpace(unitVM.Division.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Divisi tidak boleh kosong, ");
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    division = this.DbContext.Set<Division>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Name.Equals(unitVM.Division.Name));

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(unitVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(unitVM.Name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                    }

                    if (division == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Divisi tidak terdaftar di Master Divisi, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    unitVM.Division.Id = division.Id;
                    unitVM.Division.Code = division.Code;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Unit", unitVM.Code);
                    Error.Add("Divisi", unitVM.Name);
                    Error.Add("Nama", unitVM.Name);
                    Error.Add("Deskripsi", unitVM.Description);
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

        public List<Unit> GetSimple()
        {
            return this.DbSet.Select(x => new Unit()
            {
                Id = x.Id,
                Code = x.Code,
                Description = x.Description,
                Name = x.Name
            }).ToList();
        }
    }
}