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
	public class VatService : BasicService<CoreDbContext, Vat>, IMap<Vat, VatViewModel>
	{
		public VatService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override Tuple<List<Vat>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
		{
			IQueryable<Vat> Query = this.DbContext.Vat;
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
				  "Id","Name", "Rate", "Date"
				};

			Query = Query
				.Select(b => new Vat
				{
					Id = b.Id,

					Name = b.Name,
					Rate = b.Rate,
					Date = b.Date
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
			Pageable<Vat> pageable = new Pageable<Vat>(Query, Page - 1, Size);
			List<Vat> Data = pageable.Data.ToList<Vat>();

			int TotalData = pageable.TotalCount;
			 
			return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
		}

		public Vat MapToModel(VatViewModel viewModel)
		{
			Vat model = new Vat();
			PropertyCopier<VatViewModel, Vat>.Copy(viewModel, model);
			return model;
		}

		public VatViewModel MapToViewModel(Vat model)
		{
			VatViewModel viewModel = new VatViewModel();
			PropertyCopier<Vat, VatViewModel>.Copy(model, viewModel);

			return viewModel;
		}
	}
}