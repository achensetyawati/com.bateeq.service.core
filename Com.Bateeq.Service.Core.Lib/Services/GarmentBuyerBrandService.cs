using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class GarmentBuyerBrandService : BasicService<CoreDbContext, GarmentBuyerBrand>, IBasicUploadCsvService<GarmentBuyerBrandViewModel>, IMap<GarmentBuyerBrand, GarmentBuyerBrandViewModel>
    {
        public GarmentBuyerBrandService(IServiceProvider serviceProvider) :base(serviceProvider)
        {
        }

        public override Tuple<List<GarmentBuyerBrand>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentBuyerBrand> Query = this.DbContext.GarmentBuyerBrands;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name","BuyerName"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Name", "Buyers","BuyerName"
            };

            Query = Query
                .Select(b => new GarmentBuyerBrand
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    BuyerName=b.BuyerName,
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
            Pageable<GarmentBuyerBrand> pageable = new Pageable<GarmentBuyerBrand>(Query, Page - 1, Size);
            List<GarmentBuyerBrand> Data = pageable.Data.ToList<GarmentBuyerBrand>();

           int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public GarmentBuyerBrandViewModel MapToViewModel(GarmentBuyerBrand garmentBuyer)
        {
            GarmentBuyerBrandViewModel garmentBuyerVM = new GarmentBuyerBrandViewModel();

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
            garmentBuyerVM.BuyerName = garmentBuyer.BuyerName;
            garmentBuyerVM.Buyers = new GarmentBuyerViewModel
            {
                Id = garmentBuyer.BuyerId,
                Code = garmentBuyer.BuyerCode,
                Name = garmentBuyer.BuyerName
            };
            return garmentBuyerVM;
        }

        public GarmentBuyerBrand MapToModel(GarmentBuyerBrandViewModel garmentBuyerVM)
        {
            GarmentBuyerBrand garmentBuyer = new GarmentBuyerBrand();

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
            if (garmentBuyerVM.Buyers != null)
            {
                garmentBuyer.BuyerId = garmentBuyerVM.Buyers.Id;
                garmentBuyer.BuyerCode = garmentBuyerVM.Buyers.Code;
                garmentBuyer.BuyerName = garmentBuyerVM.Buyers.Name;
            }
            else
            {
                garmentBuyer.BuyerId = 0;
                garmentBuyer.BuyerCode = "";
                garmentBuyer.BuyerName = "";
            }
            return garmentBuyer;
        }
        public sealed class GarmentBuyerBrandMap : ClassMap<GarmentBuyerBrandViewModel>
        {
            public GarmentBuyerBrandMap()
            {
                Map(b => b.Code).Index(0);
                Map(b => b.Name).Index(1);
                Map(b => b.Buyers.Code).Index(2);
              
            }
        }
        private readonly List<string> Header = new List<string>()
        {
            "Kode Brand", "Nama Brand", "Kode Buyer" 
        };

        public List<string> CsvHeader => Header;
        public Tuple<bool, List<object>> UploadValidate(List<GarmentBuyerBrandViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (GarmentBuyerBrandViewModel garmentBuyerVM in Data)
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
                else if (Data.Any(d => d != garmentBuyerVM && d.Name.Equals(garmentBuyerVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }
 

                if (string.IsNullOrWhiteSpace(garmentBuyerVM.Buyers.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode Buyer tidak boleh kosong, ");
                }else
                {
                    GarmentBuyer buyer = DbContext.GarmentBuyers.FirstOrDefault(s => s.Code == garmentBuyerVM.Buyers.Code);
                    if (buyer == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode buyer tidak ada di master, ");
                    }
                    else
                    {
                        garmentBuyerVM.Buyers.Id = buyer.Id;
                        garmentBuyerVM.Buyers.Name = buyer.Name;
                    }
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
                  
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Brand", garmentBuyerVM.Code);
                    Error.Add("Nama Brand", garmentBuyerVM.Name);
                    Error.Add("Kode Buyer", garmentBuyerVM.Buyers.Code);
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

        public IQueryable<GarmentBuyerBrand> GetByName(string Keyword, string Filter)
        {
            IQueryable<GarmentBuyerBrand> Query = this.DbContext.GarmentBuyerBrands;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>("{}");

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword).Distinct();
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                  "Name"
            };

            Query = Query
                .Select(p => new GarmentBuyerBrand
                {

                    Name = p.Name,
                    Code = p.Code,
                    Id=p.Id

                });

            /* Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_updatedDate", General.DESCENDING);

                Query = Query.OrderByDescending(b => b.Name); /* Default Order */
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

            return Query.Distinct();
        }
    }
}
