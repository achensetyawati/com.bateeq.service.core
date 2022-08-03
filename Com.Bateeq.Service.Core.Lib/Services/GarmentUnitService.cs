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
	public class GarmentUnitService : BasicService<CoreDbContext, Unit> , IMap<Unit, UnitViewModel>
	{
 
		public GarmentUnitService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
		public override Tuple<List<Unit>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
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
				"Id", "Code", "Division", "Name"
			};
			IEnumerable<string> unit = new string[] { "E05", "C2A", "C2B", "C2C", "C1A", "C1B" };
			Query = from a in Query
					where (unit.Contains(a.Code))
					select new Unit {
						Id =a.Id,
						Code = a.Code,
						DivisionId = a.DivisionId,
						DivisionCode = a.DivisionCode,
						DivisionName = a.DivisionName,
						Name =a.Name
					};
		
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

			return unit;
		}

	}
}
