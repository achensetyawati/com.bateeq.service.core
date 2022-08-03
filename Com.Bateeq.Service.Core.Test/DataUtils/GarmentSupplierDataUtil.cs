using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
	public class GarmentSupplierDataUtil : BasicDataUtil<CoreDbContext, GarmentSupplierService, GarmentSupplier>, IEmptyData<GarmentSupplierViewModel>
	{
		public GarmentSupplierDataUtil(CoreDbContext dbContext, GarmentSupplierService service) : base(dbContext, service)
		{

		}
		public GarmentSupplierViewModel GetEmptyData()
		{
			GarmentSupplierViewModel Data = new GarmentSupplierViewModel();
            Data.name = string.Empty;
            Data.code = string.Empty;
            Data.usevat = true;
            Data.import = true;
            Data.usetax = true;
            Data.IncomeTaxes = null;
			
			return Data;
		}

		public override GarmentSupplier GetNewData()
		{
			string guid = Guid.NewGuid().ToString();
			GarmentSupplier TestData = new GarmentSupplier
			{
				Name = guid,
				Code = guid,
				UseVat = true,
				Import = true,
				UseTax = true,
                NPWP = guid,
                SerialNumber = guid,
                PIC = guid,
                Address = guid,
                Contact = guid,
                IncomeTaxesId = 1,
                IncomeTaxesName = guid,
                IncomeTaxesRate = 1
			};

			return TestData;
		}
        public GarmentSupplierViewModel GetNewData1()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = "",
                usetax = true,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 1,
                    name = guid,
                    rate = ""
                },
            };

            return TestData;
        }
        public GarmentSupplierViewModel GetNewData2()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = false,
                usetax = true,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 2,
                    name = guid,
                    rate = 0
                },
            };

            return TestData;
        }
        public GarmentSupplierViewModel GetNewData3()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = false,
                usetax = true,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 3,
                    name = guid,
                    rate = -1
                },
            };

            return TestData;
        }
        public GarmentSupplierViewModel GetNewData4()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = false,
                usetax = false,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 1,
                    name = "",
                    rate = 0
                },
            };

            return TestData;
        }
        public GarmentSupplierViewModel GetNewData5()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = false,
                usetax = false,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 1,
                    name = guid,
                    rate = 1
                },
            };

            return TestData;
        }
        public GarmentSupplierViewModel GetNewData6()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = false,
                usetax = false,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 1,
                    name = guid,
                    rate = 0
                },
            };

            return TestData;
        }
        public GarmentSupplierViewModel GetNewData7()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentSupplierViewModel TestData = new GarmentSupplierViewModel
            {
                name = guid,
                code = guid,
                usevat = true,
                import = false,
                usetax = false,
                NPWP = guid,
                serialNumber = guid,
                PIC = guid,
                address = guid,
                contact = guid,
                IncomeTaxes = new IncomeTaxViewModel
                {
                    Id = 1,
                    name = "",
                    rate = 1
                },
            };

            return TestData;
        }
        public override async Task<GarmentSupplier> GetTestDataAsync()
		{
			GarmentSupplier Data = GetNewData();
			await this.Service.CreateModel(Data);
			return Data;
		}
	}
}
