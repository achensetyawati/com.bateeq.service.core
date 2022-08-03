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
    public class HolidayService : BasicService<CoreDbContext, Holiday>, IMap<Holiday, HolidayViewModel>
    {
        public HolidayService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Holiday>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<Holiday> Query = this.DbContext.Holidays;
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
                "_id", "date", "name", "division", "description"
            };

            Query = Query
                .Select(h => new Holiday
                {
                    Id = h.Id,
                    Date = h.Date,
                    Name = h.Name,
                    DivisionId = h.DivisionId,
                    DivisionName = h.DivisionName,
                    Description = h.Description
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
            Pageable<Holiday> pageable = new Pageable<Holiday>(Query, Page - 1, Size);
            List<Holiday> Data = pageable.Data.ToList<Holiday>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public HolidayViewModel MapToViewModel(Holiday holiday)
        {
            HolidayViewModel holidayVM = new HolidayViewModel();
            holidayVM.division = new HolidayDivisionViewModel();

            holidayVM._id = holiday.Id;
            holidayVM.UId = holiday.UId;
            holidayVM._deleted = holiday._IsDeleted;
            holidayVM._active = holiday.Active;
            holidayVM._createdDate = holiday._CreatedUtc;
            holidayVM._createdBy = holiday._CreatedBy;
            holidayVM._createAgent = holiday._CreatedAgent;
            holidayVM._updatedDate = holiday._LastModifiedUtc;
            holidayVM._updatedBy = holiday._LastModifiedBy;
            holidayVM._updateAgent = holiday._LastModifiedAgent;
            holidayVM.code = holiday.Code;
            holidayVM.date = holiday.Date.Value.ToLocalTime();
            holidayVM.name = holiday.Name;
            holidayVM.division._id = holiday.DivisionId;
            holidayVM.division.name = holiday.DivisionName;
            holidayVM.description = holiday.Description;

            return holidayVM;
        }

        public Holiday MapToModel(HolidayViewModel holidayVM)
        {
            Holiday holiday = new Holiday();

            holiday.Id = holidayVM._id;
            holiday.UId = holidayVM.UId;
            holiday._IsDeleted = holidayVM._deleted;
            holiday.Active = holidayVM._active;
            holiday._CreatedUtc = holidayVM._createdDate;
            holiday._CreatedBy = holidayVM._createdBy;
            holiday._CreatedAgent = holidayVM._createAgent;
            holiday._LastModifiedUtc = holidayVM._updatedDate;
            holiday._LastModifiedBy = holidayVM._updatedBy;
            holiday._LastModifiedAgent = holidayVM._updateAgent;
            holiday.Code = holidayVM.code;
            holiday.Date = holidayVM.date;
            holiday.Name = holidayVM.name;

            if(!Equals(holidayVM.division, null))
            {
                holiday.DivisionId = holidayVM.division._id;
                holiday.DivisionName = holidayVM.division.name;
            }
            else
            {
                holiday.DivisionId = null;
                holiday.DivisionName = null;
            }

            holiday.Description = holidayVM.description;

            return holiday;
        }

        public override void OnCreating(Holiday model)
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