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
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class UomService : BasicService<CoreDbContext, Uom>, IBasicUploadCsvService<UomViewModel>, IMap<Uom, UomViewModel>
    {
        public UomService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Uom>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<Uom> Query = this.DbContext.UnitOfMeasurements;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Unit"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Unit"
            };

            Query = Query
                .Select(u => new Uom
                {
                    Id = u.Id,
                    Unit = u.Unit,
                    _LastModifiedUtc = u._LastModifiedUtc
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
            Pageable<Uom> pageable = new Pageable<Uom>(Query, Page - 1, Size);
            List<Uom> Data = pageable.Data.ToList<Uom>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public UomViewModel MapToViewModel(Uom uom)
        {
            UomViewModel uomVM = new UomViewModel
            {
                Id = uom.Id,
                UId = uom.UId,
                _IsDeleted = uom._IsDeleted,
                Active = uom.Active,
                _CreatedUtc = uom._CreatedUtc,
                _CreatedBy = uom._CreatedBy,
                _CreatedAgent = uom._CreatedAgent,
                _LastModifiedUtc = uom._LastModifiedUtc,
                _LastModifiedBy = uom._LastModifiedBy,
                _LastModifiedAgent = uom._LastModifiedAgent,
                Unit = uom.Unit
            };

            return uomVM;
        }

        public Uom MapToModel(UomViewModel uomVM)
        {
            Uom uom = new Uom
            {
                Id = uomVM.Id,
                UId = uomVM.UId,
                _IsDeleted = uomVM._IsDeleted,
                Active = uomVM.Active,
                _CreatedUtc = uomVM._CreatedUtc,
                _CreatedBy = uomVM._CreatedBy,
                _CreatedAgent = uomVM._CreatedAgent,
                _LastModifiedUtc = uomVM._LastModifiedUtc,
                _LastModifiedBy = uomVM._LastModifiedBy,
                _LastModifiedAgent = uomVM._LastModifiedAgent,
                Unit = uomVM.Unit
            };

            return uom;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Unit"
        };

        public List<string> CsvHeader => Header;

        public sealed class UomMap : ClassMap<UomViewModel>
        {
            public UomMap()
            {
                Map(u => u.Unit).Index(0);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<UomViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (UomViewModel uomVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(uomVM.Unit))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Unit tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != uomVM && d.Unit.Equals(uomVM.Unit)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Unit tidak boleh duplikat, ");
                }

                if(string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if(this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Unit.Equals(uomVM.Unit)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Unit tidak boleh duplikat, ");
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMessage)) /* Not Empty */
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Unit", uomVM.Unit);
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

        public List<Uom> GetSimple()
        {
            return this.DbSet.Select(x => new Uom()
            {
                Id = x.Id,
                Unit = x.Unit,
            }).ToList();
        }

        public List<Uom> GetSimpleWarpingWeaving()
        {
            return this.DbSet.Select(x => new Uom()
            {
                Id = x.Id,
                Unit = x.Unit,
            }).Where(o => o.Unit == "MTR" || o.Unit == "YARD").ToList();
        }
    }
}