using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ProductService : BasicService<CoreDbContext, Product>, IBasicUploadCsvService<ProductViewModel>, IMap<Product, ProductViewModel>
    {
        private const string UserAgent = "core-product-service";

        public ProductService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DbContext.Database.SetCommandTimeout(1000 * 60 * 2);
        }

        public override Tuple<List<Product>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Product> Query = this.DbContext.Products.AsNoTracking();
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
                "Id", "Code", "Name", "UOM", "Currency",  "Price", "Tags", "_LastModifiedUtc"
            };

            //Query = Query
            //    .Select(p => new Product
            //    {
            //        Id = p.Id,
            //        Code = p.Code,
            //        Name = p.Name,
            //        UomId = p.UomId,
            //        UomUnit = p.UomUnit,
            //        CurrencyId = p.CurrencyId,
            //        CurrencyCode = p.CurrencyCode,
            //        CurrencySymbol = p.CurrencySymbol,
            //        Price = p.Price,
            //        Tags = p.Tags,
            //        //SPPProperties = p.SPPProperties == null ? new ProductSPPProperty() : new ProductSPPProperty()
            //        //{
            //        //    ColorName = p.SPPProperties.ColorName,
            //        //    DesignCode = p.SPPProperties.DesignCode,
            //        //    DesignNumber = p.SPPProperties.DesignNumber,
            //        //    ProductionOrderId = p.SPPProperties.ProductionOrderId,
            //        //    ProductionOrderNo = p.SPPProperties.ProductionOrderNo,
            //        //    BuyerAddress = p.SPPProperties.BuyerAddress,
            //        //    BuyerId = p.SPPProperties.BuyerId,
            //        //    BuyerName = p.SPPProperties.BuyerName,
            //        //    Weight = p.SPPProperties.Weight,
            //        //    Construction = p.SPPProperties.Construction,
            //        //    Grade = p.SPPProperties.Grade,
            //        //    Length = p.SPPProperties.Length,
            //        //    Lot = p.SPPProperties.Lot,
            //        //    OrderTypeCode = p.SPPProperties.OrderTypeCode,
            //        //    OrderTypeId = p.SPPProperties.OrderTypeId,
            //        //    OrderTypeName = p.SPPProperties.OrderTypeName
            //        //},
            //        _LastModifiedUtc = p._LastModifiedUtc
            //    }).AsNoTracking();

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
            //Pageable<Product> pageable = new Pageable<Product>(Query, Page - 1, Size);

            var totalData = Query.Count();
            Query = Query.Skip((Page - 1) * Size).Take(Size);

            List<Product> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public Tuple<List<Product>, int, Dictionary<string, string>, List<string>> ReadModelNullTags(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Product> Query = this.DbContext.Products.Where(x => string.IsNullOrEmpty(x.Tags) || string.IsNullOrWhiteSpace(x.Tags)).AsNoTracking();
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
                "Id", "Code", "Name", "UOM", "Currency",  "Price", "Tags", "_LastModifiedUtc"
            };



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
            //Pageable<Product> pageable = new Pageable<Product>(Query, Page - 1, Size);

            var totalData = Query.Count();
            Query = Query.Skip((Page - 1) * Size).Take(Size);

            List<Product> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public ProductViewModel MapToViewModel(Product product)
        {
            ProductViewModel productVM = new ProductViewModel
            {
                Id = product.Id,
                UId = product.UId,
                _IsDeleted = product._IsDeleted,
                Active = product.Active,
                _CreatedUtc = product._CreatedUtc,
                _CreatedBy = product._CreatedBy,
                _CreatedAgent = product._CreatedAgent,
                _LastModifiedUtc = product._LastModifiedUtc,
                _LastModifiedBy = product._LastModifiedBy,
                _LastModifiedAgent = product._LastModifiedAgent,
                Code = product.Code,
                Name = product.Name,
                Price = product.Price,

                Currency = new ProductCurrencyViewModel
                {
                    Id = product.CurrencyId,
                    Code = product.CurrencyCode,
                    Symbol = product.CurrencySymbol
                },
                Description = product.Description,
                UOM = new ProductUomViewModel
                {
                    Id = product.UomId,
                    Unit = product.UomUnit
                },
                Tags = product.Tags,
                SPPProperties = product.SPPProperties == null ? null : new ProductSPPPropertyViewModel()
                {
                    BuyerAddress = product.SPPProperties.BuyerAddress,
                    BuyerId = product.SPPProperties.BuyerId,
                    BuyerName = product.SPPProperties.BuyerName,
                    ColorName = product.SPPProperties.ColorName,
                    Construction = product.SPPProperties.Construction,
                    DesignCode = product.SPPProperties.DesignCode,
                    DesignNumber = product.SPPProperties.DesignNumber,
                    Grade = product.SPPProperties.Grade,
                    Length = product.SPPProperties.Length,
                    Lot = product.SPPProperties.Lot,
                    ProductionOrderId = product.SPPProperties.ProductionOrderId,
                    ProductionOrderNo = product.SPPProperties.ProductionOrderNo,
                    Weight = product.SPPProperties.Weight,
                    OrderType = new OrderTypeViewModel()
                    {
                        Id = product.SPPProperties.OrderTypeId,
                        Code = product.SPPProperties.OrderTypeCode,
                        Name = product.SPPProperties.OrderTypeName
                    }
                }
            };

            return productVM;
        }

        public Product MapToModel(ProductViewModel productVM)
        {
            Product product = new Product
            {
                Id = productVM.Id,
                UId = productVM.UId,
                _IsDeleted = productVM._IsDeleted,
                Active = productVM.Active,
                _CreatedUtc = productVM._CreatedUtc,
                _CreatedBy = productVM._CreatedBy,
                _CreatedAgent = productVM._CreatedAgent,
                _LastModifiedUtc = productVM._LastModifiedUtc,
                _LastModifiedBy = productVM._LastModifiedBy,
                _LastModifiedAgent = productVM._LastModifiedAgent,
                Code = productVM.Code,
                Name = productVM.Name,
                Price = !Equals(productVM.Price, null) ? Convert.ToDecimal(productVM.Price) : 0, /* Check Null */
                SPPProperties = productVM.SPPProperties == null ? null : new ProductSPPProperty()
                {
                    Id = productVM.Id,
                    BuyerAddress = productVM.SPPProperties.BuyerAddress,
                    BuyerId = productVM.SPPProperties.BuyerId,
                    BuyerName = productVM.SPPProperties.BuyerName,
                    ColorName = productVM.SPPProperties.ColorName,
                    Construction = productVM.SPPProperties.Construction,
                    DesignCode = productVM.SPPProperties.DesignCode,
                    DesignNumber = productVM.SPPProperties.DesignNumber,
                    Grade = productVM.SPPProperties.Grade,
                    Length = productVM.SPPProperties.Length,
                    Lot = productVM.SPPProperties.Lot,
                    OrderTypeCode = productVM.SPPProperties.OrderType == null ? null : productVM.SPPProperties.OrderType.Code,
                    OrderTypeId = productVM.SPPProperties.OrderType == null ? 0 : productVM.SPPProperties.OrderType.Id,
                    OrderTypeName = productVM.SPPProperties.OrderType == null ? null : productVM.SPPProperties.OrderType.Name,
                    ProductionOrderId = productVM.SPPProperties.ProductionOrderId,
                    ProductionOrderNo = productVM.SPPProperties.ProductionOrderNo,
                    Weight = productVM.SPPProperties.Weight

                }
            };

            if (!Equals(productVM.Currency, null))
            {
                product.CurrencyId = productVM.Currency.Id;
                product.CurrencyCode = productVM.Currency.Code;
                product.CurrencySymbol = productVM.Currency.Symbol;
            }
            else
            {
                product.CurrencyId = null;
                product.CurrencyCode = null;
            }

            product.Description = productVM.Description;

            if (!Equals(productVM.UOM, null))
            {
                product.UomId = productVM.UOM.Id;
                product.UomUnit = productVM.UOM.Unit;
            }
            else
            {
                product.UomId = null;
                product.UomUnit = null;
            }

            product.Tags = productVM.Tags;

            return product;
        }

        public List<string> CsvHeader { get; } = new List<string>()
        {
            "Kode Barang", "Nama Barang", "Satuan", "Mata Uang", "Harga", "Tags", "Keterangan"
        };

        public sealed class ProductMap : ClassMap<ProductViewModel>
        {
            public ProductMap()
            {
                Map(p => p.Code).Index(0);
                Map(p => p.Name).Index(1);
                Map(p => p.UOM.Unit).Index(2);
                Map(p => p.Currency.Code).Index(3);
                Map(p => p.Price).Index(4).TypeConverter<StringConverter>();
                Map(p => p.Tags).Index(5);
                Map(p => p.Description).Index(6);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<ProductViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            Currency currency = null;
            Uom uom = null;

            foreach (ProductViewModel productVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(productVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productVM && d.Code.Equals(productVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productVM && d.Name.Equals(productVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.UOM.Unit))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Currency.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak boleh kosong, ");
                }

                decimal Price = 0;
                if (string.IsNullOrWhiteSpace(productVM.Price))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga tidak boleh kosong, ");
                }
                else if (!decimal.TryParse(productVM.Price, out Price))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga harus numerik, ");
                }
                else if (Price < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga harus lebih besar dari 0, ");
                }
                else
                {
                    string[] PriceSplit = productVM.Price.Split('.');
                    if (PriceSplit.Count().Equals(2) && PriceSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Harga maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    currency = this.DbContext.Set<Currency>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Code.Equals(productVM.Currency.Code));
                    uom = this.DbContext.Set<Uom>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Unit.Equals(productVM.UOM.Unit));

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(productVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(productVM.Name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                    }

                    if (currency == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak terdaftar dalam master Mata Uang, ");
                    }

                    if (uom == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak terdaftar dalam master Satuan, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    productVM.Price = Price;
                    productVM.Currency.Id = currency.Id;
                    productVM.UOM.Id = uom.Id;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Barang", productVM.Code);
                    Error.Add("Nama Barang", productVM.Name);
                    Error.Add("Satuan", productVM.UOM.Unit);
                    Error.Add("Mata Uang", productVM.Currency.Code);
                    Error.Add("Harga", productVM.Price);
                    Error.Add("Tags", productVM.Tags);
                    Error.Add("Keterangan", productVM.Description);
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

        public List<Product> GetByIds(List<string> ids)
        {
            return this.DbSet.Include(x => x.SPPProperties).Where(p => ids.Contains(p.Id.ToString()) && p._IsDeleted == false)
                .ToList();
        }

        public List<Product> GetSimple()
        {
            return DbSet.IgnoreQueryFilters().Select(x => new Product()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();
        }

        public override Task<Product> ReadModelById(int Id)
        {
            //base.DbContext.Set<ProductSPPProperty>().Load();
            return DbSet.Include(x => x.SPPProperties).AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> CreateProduct(PackingModel packings)
        {
            //DbContext.Database.SetCommandTimeout(1000 * 60 * 10);

            var packingProducts = packings.PackingDetails.Select(packingDetail => string.Format("{0}/{1}/{2}/{3}/{4}/{5}", packings.ProductionOrderNo, packings.ColorName, packings.Construction,
                                packingDetail.Lot, packingDetail.Grade, packingDetail.Length) +
                                (string.IsNullOrWhiteSpace(packingDetail.Remark) ? "" : string.Format("/{0}", packingDetail.Remark))).ToList();

            //var productNames = (from packingDetail in packings.PackingDetails
            //                    select )
            //                    .Except((from product in DbSet where product._IsDeleted == false select product.Name));

            var uom = DbContext.UnitOfMeasurements.FirstOrDefault(f => f.Unit.Equals(packings.PackingUom));
            if (uom == null)
            {
                uom = new Uom
                {
                    Active = true,
                    Unit = packings.PackingUom,
                    _IsDeleted = false,
                    _CreatedBy = this.Username,
                    _CreatedUtc = DateTimeOffset.Now.DateTime,
                    _CreatedAgent = UserAgent
                };
                DbContext.UnitOfMeasurements.Add(uom);
                await DbContext.SaveChangesAsync();

                //uomId = uom.Id;
            }

            var tags = string.Format("sales contract #{0}", packings.SalesContractNo);
            CodeGenerator codeGenerator = new CodeGenerator();
            var listProductToCreate = new List<Product>();
            foreach (var packingDetail in packings.PackingDetails)
            {
                var packingProduct = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", packings.ProductionOrderNo, packings.ColorName, packings.Construction, packingDetail.Lot, packingDetail.Grade, packingDetail.Length) + (string.IsNullOrWhiteSpace(packingDetail.Remark) ? "" : string.Format("/{0}", packingDetail.Remark));

                var existingProduct = DbContext.Products.FirstOrDefault(f => f.Name.Equals(packingProduct));

                if (existingProduct == null)
                {
                    var productToCreate = new Product()
                    {
                        Active = true,
                        Code = codeGenerator.GenerateCode(),
                        Name = packingProduct,
                        UomId = uom.Id,
                        UomUnit = packings.PackingUom,
                        Tags = tags,
                        SPPProperties = new ProductSPPProperty()
                        {
                            ProductionOrderId = packings.ProductionOrderId,
                            ProductionOrderNo = packings.ProductionOrderNo,
                            DesignCode = packings.DesignCode,
                            DesignNumber = packings.DesignNumber,
                            OrderTypeId = packings.OrderTypeId,
                            OrderTypeCode = packings.OrderTypeCode,
                            OrderTypeName = packings.OrderTypeName,
                            BuyerId = packings.BuyerId,
                            BuyerName = packings.BuyerName,
                            BuyerAddress = packings.BuyerAddress,
                            ColorName = packings.ColorName,
                            Construction = packings.Construction,
                            Lot = packingDetail.Lot,
                            Grade = packingDetail.Grade,
                            Weight = packingDetail.Weight,
                            Length = packingDetail.Length
                        },
                        _IsDeleted = false,
                        _CreatedBy = this.Username,
                        _CreatedUtc = DateTimeOffset.Now.DateTime,
                        _CreatedAgent = UserAgent
                    };

                    listProductToCreate.Add(productToCreate);
                }

            }

            if (listProductToCreate.Count > 0)
                await DbContext.AddRangeAsync(listProductToCreate);


            var rowAffected = await DbContext.SaveChangesAsync();
            if (rowAffected > 0)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        public Task<List<ProductViewModel>> GetProductByProductionOrderNo(string productionOrderNo)
        {
            var product = DbContext.ProductSPPProperties.Include(x => x.Product).Where(x => x.ProductionOrderNo == productionOrderNo);

            return product.Select(x => new ProductViewModel()
            {
                Active = x.Product.Active,
                Code = x.Product.Code,
                Currency = new ProductCurrencyViewModel()
                {
                    Code = x.Product.CurrencyCode,
                    Id = x.Product.CurrencyId,
                    Symbol = x.Product.CurrencySymbol
                },
                Description = x.Product.Description,
                Id = x.Product.Id,
                Name = x.Product.Name,
                Price = x.Product.Price,
                Tags = x.Product.Tags,
                UId = x.Product.UId,
                UOM = new ProductUomViewModel()
                {
                    Id = x.Product.UomId,
                    Unit = x.Product.UomUnit
                },
                _LastModifiedUtc = x.Product._LastModifiedUtc,
                SPPProperties = new ProductSPPPropertyViewModel()
                {
                    BuyerAddress = x.BuyerAddress,
                    BuyerId = x.BuyerId,
                    BuyerName = x.BuyerName,
                    ColorName = x.ColorName,
                    Construction = x.Construction,
                    DesignCode = x.DesignCode,
                    DesignNumber = x.DesignNumber,
                    Grade = x.Grade,
                    Length = x.Length,
                    Lot = x.Lot,
                    OrderType = new OrderTypeViewModel()
                    {
                        Code = x.OrderTypeCode,
                        Id = x.OrderTypeId,
                        Name = x.OrderTypeName
                    },
                    ProductionOrderId = x.ProductionOrderId,
                    ProductionOrderNo = x.ProductionOrderNo,
                    Weight = x.Weight
                }
            }).ToListAsync();
        }

        public Task<Product> GetProductByName(string productName)
        {
            return DbSet.FirstOrDefaultAsync(f => f.Name.Equals(productName));
        }

        public Task<Product> GetProductForSpinning(int Id)
        {
            return DbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}