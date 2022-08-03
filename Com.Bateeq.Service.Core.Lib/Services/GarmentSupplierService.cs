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
using CsvHelper.Configuration;
using System.Dynamic;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
	public class GarmentSupplierService : BasicService<CoreDbContext, GarmentSupplier>, IBasicUploadCsvService<GarmentSupplierViewModel>, IMap<GarmentSupplier, GarmentSupplierViewModel>
	{

		private readonly string[] ImportAllowed = { "True", "False" };
		private readonly string[] UseVatAllowed = { "True", "False" };
		private readonly string[] UseTaxAllowed = { "True", "False" };
		public GarmentSupplierService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
		public override Tuple<List<GarmentSupplier>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
		{
			IQueryable<GarmentSupplier> Query = this.DbContext.GarmentSuppliers;
			Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
			Query = ConfigureFilter(Query, FilterDictionary);
			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
			/* Search With Keyword */
			if (Keyword != null)
			{
				List<string> SearchAttributes = new List<string>()
				{
					"Code", "Name","Address"
				};

				Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
			}

			/* Const Select */
			List<string> SelectedFields = new List<string>()
			{
				"Id", "code", "name", "address", "import", "NPWP", "usevat", "usetax", "IncomeTaxes"
			};

			Query = Query
				.Select(s => new GarmentSupplier
				{
					Id = s.Id,
					Code = s.Code,
					Name = s.Name,
					Address = s.Address,
					Import = s.Import,
					NPWP = s.NPWP,
					UseVat = s.UseVat,
					UseTax = s.UseTax,
					IncomeTaxesId = s.IncomeTaxesId,
					IncomeTaxesName = s.IncomeTaxesName,
					IncomeTaxesRate = s.IncomeTaxesRate,
					_LastModifiedUtc =s._LastModifiedUtc
				}).OrderByDescending(b => b._LastModifiedUtc);

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

				Query = OrderType.Equals(General.ASCENDING ) ?
					Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
					Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
			}

			/* Pagination */
			Pageable<GarmentSupplier> pageable = new Pageable<GarmentSupplier>(Query , Page - 1, Size);
			List<GarmentSupplier> Data = pageable.Data.ToList<GarmentSupplier>();

			int TotalData = pageable.TotalCount;

			return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
		}
		public GarmentSupplierViewModel MapToViewModel(GarmentSupplier GarmentSupplier)
		{
			GarmentSupplierViewModel GarmentSupplierVM = new GarmentSupplierViewModel();

			GarmentSupplierVM.Id = GarmentSupplier.Id;
			GarmentSupplierVM.UId = GarmentSupplier.UId;
			GarmentSupplierVM._IsDeleted = GarmentSupplier._IsDeleted;
			GarmentSupplierVM.Active = GarmentSupplier.Active;
			GarmentSupplierVM._CreatedUtc = GarmentSupplier._CreatedUtc;
			GarmentSupplierVM._CreatedBy = GarmentSupplier._CreatedBy;
			GarmentSupplierVM._CreatedAgent = GarmentSupplier._CreatedAgent;
			GarmentSupplierVM._LastModifiedUtc = GarmentSupplier._LastModifiedUtc;
			GarmentSupplierVM._LastModifiedBy = GarmentSupplier._LastModifiedBy;
			GarmentSupplierVM._LastModifiedAgent = GarmentSupplier._LastModifiedAgent;
			GarmentSupplierVM.code = GarmentSupplier.Code;
			GarmentSupplierVM.name = GarmentSupplier.Name;
			GarmentSupplierVM.address = GarmentSupplier.Address;
			GarmentSupplierVM.contact = GarmentSupplier.Contact;
			GarmentSupplierVM.PIC = GarmentSupplier.PIC;
			GarmentSupplierVM.import = GarmentSupplier.Import;
			GarmentSupplierVM.usevat = GarmentSupplier.UseVat;
			GarmentSupplierVM.usetax = GarmentSupplier.UseTax;
			GarmentSupplierVM.IncomeTaxes = new IncomeTaxViewModel
			{
				Id = GarmentSupplier.IncomeTaxesId,
				name = GarmentSupplier.IncomeTaxesName,
				rate = GarmentSupplier.IncomeTaxesRate
			};
			GarmentSupplierVM.NPWP = GarmentSupplier.NPWP;
			GarmentSupplierVM.serialNumber = GarmentSupplier.SerialNumber;
			

			return GarmentSupplierVM;
		}
		public GarmentSupplier MapToModel(GarmentSupplierViewModel GarmentSupplierVM)
		{
			GarmentSupplier GarmentSupplier = new GarmentSupplier();

			GarmentSupplier.Id = GarmentSupplierVM.Id;
			GarmentSupplier.UId = GarmentSupplierVM.UId;
			GarmentSupplier._IsDeleted = GarmentSupplierVM._IsDeleted;
			GarmentSupplier.Active = GarmentSupplierVM.Active;
			GarmentSupplier._CreatedUtc = GarmentSupplierVM._CreatedUtc;
			GarmentSupplier._CreatedBy = GarmentSupplierVM._CreatedBy;
			GarmentSupplier._CreatedAgent = GarmentSupplierVM._CreatedAgent;
			GarmentSupplier._LastModifiedUtc = GarmentSupplierVM._LastModifiedUtc;
			GarmentSupplier._LastModifiedBy = GarmentSupplierVM._LastModifiedBy;
			GarmentSupplier._LastModifiedAgent = GarmentSupplierVM._LastModifiedAgent;
			GarmentSupplier.Code = GarmentSupplierVM.code;
			GarmentSupplier.Name = GarmentSupplierVM.name;
			GarmentSupplier.Address = GarmentSupplierVM.address;
			GarmentSupplier.Contact = GarmentSupplierVM.contact;
			GarmentSupplier.PIC = GarmentSupplierVM.PIC;
			GarmentSupplier.Import = !Equals(GarmentSupplierVM.import, null) ? Convert.ToBoolean(GarmentSupplierVM.import) : false;
			GarmentSupplier.UseVat = !Equals(GarmentSupplierVM.usevat, null) ? Convert.ToBoolean(GarmentSupplierVM.usevat) : false;
			GarmentSupplier.UseTax = !Equals(GarmentSupplierVM.usetax, null) ? Convert.ToBoolean(GarmentSupplierVM.usetax) : false; /* Check Null */
			if (GarmentSupplierVM.IncomeTaxes != null)
			{
				GarmentSupplier.IncomeTaxesId = GarmentSupplierVM.IncomeTaxes.Id;
				GarmentSupplier.IncomeTaxesName = GarmentSupplierVM.IncomeTaxes.name;
				GarmentSupplier.IncomeTaxesRate = !Equals(GarmentSupplierVM.IncomeTaxes.rate, null) ? Convert.ToDouble(GarmentSupplierVM.IncomeTaxes.rate) : null;
            }
			else
			{
				GarmentSupplier.IncomeTaxesId = 1;
				GarmentSupplier.IncomeTaxesName = "";
				GarmentSupplier.IncomeTaxesRate = 0;
			}
			GarmentSupplier.NPWP = GarmentSupplierVM.NPWP;
			GarmentSupplier.SerialNumber = GarmentSupplierVM.serialNumber;
			

			return GarmentSupplier;
		}

		/* Upload CSV */
		private readonly List<string> Header = new List<string>()
		{
			"Kode", "Nama Supplier", "Alamat", "Kontak", "PIC", "Import","Kena PPN","Kena PPH", "Jenis PPH", "Rate PPH", "NPWP", "Serial Number"
		};
		public List<string> CsvHeader => Header;

		public sealed class GarmentSupplierMap : ClassMap<GarmentSupplierViewModel>
		{
			public GarmentSupplierMap()
			{

				Map(s => s.code).Index(0);
				Map(s => s.name).Index(1);
				Map(s => s.address).Index(2);
				Map(s => s.contact).Index(3);
				Map(s => s.PIC).Index(4);
				Map(s => s.import).Index(5).TypeConverter<StringConverter>();
				Map(s => s.usevat ).Index(6).TypeConverter<StringConverter>();
				Map(s => s.usetax).Index(7).TypeConverter<StringConverter>();
				Map(s => s.IncomeTaxes.name).Index(8);
				Map(s => s.IncomeTaxes.rate).Index(9).TypeConverter<StringConverter>();
				Map(s => s.NPWP).Index(10);
				Map(s => s.serialNumber).Index(11);
			}
		}

		public Tuple<bool, List<object>> UploadValidate(List<GarmentSupplierViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
		{
			List<object> ErrorList = new List<object>();
			string ErrorMessage;
			bool Valid = true;
			IncomeTax incomeTax = null;

			foreach (GarmentSupplierViewModel GarmentSupplierVM in Data)
			{
				ErrorMessage = "";

				if (string.IsNullOrWhiteSpace(GarmentSupplierVM.code))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
				}
				else if (Data.Any(d => d != GarmentSupplierVM && d.code.Equals(GarmentSupplierVM.code)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
				}

				if (string.IsNullOrWhiteSpace(GarmentSupplierVM.name))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
				}

				if (string.IsNullOrWhiteSpace(Convert.ToString(GarmentSupplierVM.import)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Import tidak boleh kosong, ");
				}
				else if (!ImportAllowed.Any(i => i.Equals(Convert.ToString(GarmentSupplierVM.import), StringComparison.CurrentCultureIgnoreCase)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Import harus diisi dengan True atau False, ");
				}
				if (string.IsNullOrWhiteSpace(Convert.ToString(GarmentSupplierVM.usevat)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kena PPN tidak boleh kosong, ");
				}
				else if (!UseVatAllowed.Any(i => i.Equals(Convert.ToString(GarmentSupplierVM.usevat), StringComparison.CurrentCultureIgnoreCase)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kena PPN harus diisi dengan True atau False, ");
				}
				if (string.IsNullOrWhiteSpace(Convert.ToString(GarmentSupplierVM.usetax)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kena PPH tidak boleh kosong, ");
				}
				else if (!UseTaxAllowed.Any(i => i.Equals(Convert.ToString(GarmentSupplierVM.usetax), StringComparison.CurrentCultureIgnoreCase)))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kena PPH harus diisi dengan True atau False, ");
                }
                bool tax;
                bool.TryParse(Convert.ToString(GarmentSupplierVM.usetax), out tax);
                double Rate = 0;
                var isIncometaxRateNumber = double.TryParse(Convert.ToString(GarmentSupplierVM.IncomeTaxes.rate), out Rate);
                if (tax == true)
                {
                    if (string.IsNullOrWhiteSpace(GarmentSupplierVM.IncomeTaxes.name))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Jenis PPH tidak boleh kosong, ");
                    }
                    string[] RateSplit = Convert.ToString(GarmentSupplierVM.IncomeTaxes.rate).Split('.');
                    
                    if (string.IsNullOrWhiteSpace(Convert.ToString(GarmentSupplierVM.IncomeTaxes.rate)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Rate PPH tidak boleh kosong, ");
                    }
                    else if (!isIncometaxRateNumber)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Rate PPH harus numerik, ");
                    }
                    else if (Rate < 0 || Rate == 0)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Rate PPH harus lebih besar dari 0, ");
                    }
                    else if (RateSplit.Count().Equals(2) && RateSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kurs maksimal memiliki 2 digit dibelakang koma, ");
                    }
                    IncomeTax suppliers = DbContext.IncomeTaxes.FirstOrDefault(s => s.Name == GarmentSupplierVM.IncomeTaxes.name && s.Rate == Rate);
                    if (suppliers == null)
                    {
                        IncomeTax incometaxesname = DbContext.IncomeTaxes.FirstOrDefault(s => s.Name == GarmentSupplierVM.IncomeTaxes.name);
                        if (incometaxesname == null && GarmentSupplierVM.IncomeTaxes.name != "")
                        {
                            ErrorMessage = string.Concat(ErrorMessage, "Jenis PPH Tidak Ada di Master PPH, ");
                        }
                        IncomeTax incometaxesrate = DbContext.IncomeTaxes.FirstOrDefault(s => s.Rate == Rate);
                        if (incometaxesrate == null && Rate != 0)
                        {
                            ErrorMessage = string.Concat(ErrorMessage, "Rate PPH Tidak Ada di Master PPH, ");
                        }
                        if (incometaxesrate != null && incometaxesname != null)
                        {
                            ErrorMessage = string.Concat(ErrorMessage, " Jenis PPH dan Rate PPH tidak ada di Master PPH, ");
                        }

                    }
                    else
                    {
                        GarmentSupplierVM.IncomeTaxes.Id = suppliers.Id;
                        GarmentSupplierVM.IncomeTaxes.name = suppliers.Name;
                        GarmentSupplierVM.IncomeTaxes.rate = suppliers.Rate;
                    }
                }
                else if (tax == false)
                {
                    if (GarmentSupplierVM.IncomeTaxes.name != "" && Rate != 0)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, " Jenis PPH / Rate PPH harus kosong, ");
                    }
                    else if(GarmentSupplierVM.IncomeTaxes.name != "")
                    {
                        ErrorMessage = string.Concat(ErrorMessage, " Jenis PPH harus kosong, ");
                    }
                    else if (Rate != 0)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, " Rate PPH harus kosong, ");
                    }
                    else
                    {
                        GarmentSupplierVM.IncomeTaxes.Id = 1;
                        GarmentSupplierVM.IncomeTaxes.name = "";
                        GarmentSupplierVM.IncomeTaxes.rate = 0;
                    }
                    
                }

                
                if (string.IsNullOrEmpty(ErrorMessage))
				{
					/* Service Validation */
					incomeTax = this.DbContext.Set<IncomeTax>().FirstOrDefault(d => d._IsDeleted.Equals(false) );
					if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(GarmentSupplierVM.code)))
					{
						ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
					}
					if (incomeTax==null)
					{
						ErrorMessage = string.Concat(ErrorMessage, "PPH tidak terdaftar dalam master Income Tax");
					}
				}

				if (string.IsNullOrEmpty(ErrorMessage))
				{
					GarmentSupplierVM.import = Convert.ToBoolean(GarmentSupplierVM.import);
				}
				else
				{
					ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
					var Error = new ExpandoObject() as IDictionary<string, object>;

					Error.Add("Kode", GarmentSupplierVM.code);
					Error.Add("Nama Supplier", GarmentSupplierVM.name);
					Error.Add("Alamat", GarmentSupplierVM.address);
					Error.Add("Kontak", GarmentSupplierVM.code);
					Error.Add("PIC", GarmentSupplierVM.PIC);
					Error.Add("Import", GarmentSupplierVM.import);
					Error.Add("Kena PPN", GarmentSupplierVM.usevat);
					Error.Add("Kena PPH", GarmentSupplierVM.usetax);
					Error.Add("Jenis PPH", GarmentSupplierVM.IncomeTaxes.name);
					Error.Add("Rate PPH", GarmentSupplierVM.IncomeTaxes.rate);
					Error.Add("NPWP", GarmentSupplierVM.NPWP);
					Error.Add("Serial Number", GarmentSupplierVM.serialNumber);
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
