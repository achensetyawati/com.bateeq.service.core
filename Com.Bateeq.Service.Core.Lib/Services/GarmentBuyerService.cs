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
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class GarmentBuyerService : BasicService<CoreDbContext, GarmentBuyer>, IBasicUploadCsvService<GarmentBuyerViewModel>, IMap<GarmentBuyer, GarmentBuyerViewModel>
    {
        private readonly string[] Types = { "Lokal", "Ekspor", "Internal" };
        private readonly string[] Countries = { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua and Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Canada", "Cape Verde", "Cayman Islands", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cruise Ship", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Kyrgyz Republic", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "Norway", "Oman", "Pakistan", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre and Miquelon", "Samoa", "San Marino", "Satellite", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sri Lanka", "St Kitts and Nevis", "St Lucia", "St Vincent", "St. Lucia", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks and Caicos", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States of America", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe" };

        public GarmentBuyerService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<GarmentBuyer>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<GarmentBuyer> Query = this.DbContext.GarmentBuyers;
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
                "Id", "Code", "Name", "Address", "City", "Country", "Contact", "Tempo", "_LastModifiedUtc", "Type"
            };

            Query = Query
                .Select(b => new GarmentBuyer
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Address = b.Address,
                    City = b.City,
                    Country = b.Country,
                    Contact = b.Contact,
                    Tempo = b.Tempo,
                    Type = b.Type,
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
            Pageable<GarmentBuyer> pageable = new Pageable<GarmentBuyer>(Query, Page - 1, Size);
            List<GarmentBuyer> Data = pageable.Data.ToList<GarmentBuyer>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public GarmentBuyerViewModel MapToViewModel(GarmentBuyer garmentBuyer)
        {
            GarmentBuyerViewModel garmentBuyerVM = new GarmentBuyerViewModel();

            garmentBuyerVM.Id = garmentBuyer.Id;
            garmentBuyerVM.UId = garmentBuyer.UId;
            garmentBuyerVM._IsDeleted = garmentBuyer._IsDeleted;
            garmentBuyerVM.Active = garmentBuyer.Active;
            garmentBuyerVM._CreatedUtc = garmentBuyer._CreatedUtc;
            garmentBuyerVM._CreatedBy = garmentBuyer._CreatedBy;
            garmentBuyerVM._CreatedAgent = garmentBuyer._CreatedAgent;
            garmentBuyerVM._LastModifiedUtc = garmentBuyer._LastModifiedUtc;
            garmentBuyerVM._LastModifiedBy = garmentBuyer._LastModifiedBy;
            garmentBuyerVM._LastModifiedAgent = garmentBuyer._LastModifiedAgent;
            garmentBuyerVM.Code = garmentBuyer.Code;
            garmentBuyerVM.Name = garmentBuyer.Name;
            garmentBuyerVM.Address = garmentBuyer.Address;
            garmentBuyerVM.City = garmentBuyer.City;
            garmentBuyerVM.Country = garmentBuyer.Country;
            garmentBuyerVM.Contact = garmentBuyer.Contact;
            garmentBuyerVM.Tempo = garmentBuyer.Tempo;
            garmentBuyerVM.Type = garmentBuyer.Type;
            garmentBuyerVM.NPWP = garmentBuyer.NPWP;

            return garmentBuyerVM;
        }

        public GarmentBuyer MapToModel(GarmentBuyerViewModel garmentBuyerVM)
        {
            GarmentBuyer garmentBuyer = new GarmentBuyer();

            garmentBuyer.Id = garmentBuyerVM.Id;
            garmentBuyer.UId = garmentBuyerVM.UId;
            garmentBuyer._IsDeleted = garmentBuyerVM._IsDeleted;
            garmentBuyer.Active = garmentBuyerVM.Active;
            garmentBuyer._CreatedUtc = garmentBuyerVM._CreatedUtc;
            garmentBuyer._CreatedBy = garmentBuyerVM._CreatedBy;
            garmentBuyer._CreatedAgent = garmentBuyerVM._CreatedAgent;
            garmentBuyer._LastModifiedUtc = garmentBuyerVM._LastModifiedUtc;
            garmentBuyer._LastModifiedBy = garmentBuyerVM._LastModifiedBy;
            garmentBuyer._LastModifiedAgent = garmentBuyerVM._LastModifiedAgent;
            garmentBuyer.Code = garmentBuyerVM.Code;
            garmentBuyer.Name = garmentBuyerVM.Name;
            garmentBuyer.Address = garmentBuyerVM.Address;
            garmentBuyer.City = garmentBuyerVM.City;
            garmentBuyer.Country = garmentBuyerVM.Country;
            garmentBuyer.Contact = garmentBuyerVM.Contact;
            garmentBuyer.Tempo = !Equals(garmentBuyerVM.Tempo, null) ? Convert.ToInt32(garmentBuyerVM.Tempo) : null; /* Check Null */
            garmentBuyer.Type = garmentBuyerVM.Type;
            garmentBuyer.NPWP = garmentBuyerVM.NPWP;

            return garmentBuyer;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode Buyer", "Nama", "Alamat", "Kota", "Negara", "NPWP", "Jenis Buyer", "Kontak", "Tempo"
        };

        public List<string> CsvHeader => Header;

        public sealed class GarmentBuyerMap : ClassMap<GarmentBuyerViewModel>
        {
            public GarmentBuyerMap()
            {
                Map(b => b.Code).Index(0);
                Map(b => b.Name).Index(1);
                Map(b => b.Address).Index(2);
                Map(b => b.City).Index(3);
                Map(b => b.Country).Index(4);
                Map(b => b.NPWP).Index(5);
                Map(b => b.Type).Index(6);
                Map(b => b.Contact).Index(7);
                Map(b => b.Tempo).Index(8).TypeConverter<StringConverter>();
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<GarmentBuyerViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (GarmentBuyerViewModel garmentBuyerVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(garmentBuyerVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != garmentBuyerVM && d.Code.Equals(garmentBuyerVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(garmentBuyerVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(garmentBuyerVM.Type))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Jenis Buyer tidak boleh kosong, ");
                }
                else if (!Types.Any(t => t.Equals(garmentBuyerVM.Type)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Jenis Buyer harus salah satu dari Lokal, Ekspor, Internal; ");
                }

                if (string.IsNullOrWhiteSpace(garmentBuyerVM.Country))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Negara tidak boleh kosong, ");
                }
                else if (!Countries.Any(c => c.Equals(garmentBuyerVM.Country, StringComparison.CurrentCultureIgnoreCase)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Negara tidak terdaftar di list Negara, ");
                }

                int Tempo = 0;
                if (string.IsNullOrWhiteSpace(garmentBuyerVM.Tempo))
                {
                    garmentBuyerVM.Tempo = 0;
                }
                else if (!int.TryParse(garmentBuyerVM.Tempo, out Tempo))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Tempo harus angka, ");
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(garmentBuyerVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    garmentBuyerVM.Tempo = Tempo;
                    garmentBuyerVM.Country = Countries.First(c => c.Equals(garmentBuyerVM.Country, StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Buyer", garmentBuyerVM.Code);
                    Error.Add("Nama", garmentBuyerVM.Name);
                    Error.Add("Alamat", garmentBuyerVM.Address);
                    Error.Add("Kota", garmentBuyerVM.City);
                    Error.Add("Negara", garmentBuyerVM.Country);
                    Error.Add("NPWP", garmentBuyerVM.NPWP);
                    Error.Add("Jenis Buyer", garmentBuyerVM.Type);
                    Error.Add("Kontak", garmentBuyerVM.Contact);
                    Error.Add("Tempo", garmentBuyerVM.Tempo);
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