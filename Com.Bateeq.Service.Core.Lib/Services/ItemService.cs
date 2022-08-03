using Com.DanLiris.Service.Core.Lib.Helpers;
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
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Com.Moonlay.Models;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ItemService : BasicService<CoreDbContext, Item>,  IMap<Item, ItemViewModel>
    {

        private const string UserAgent = "core-item-service";
        private readonly ItemService itemService;
        //public readonly IServiceProvider ServiceProvider;

        public ItemService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DbContext.Database.SetCommandTimeout(1000 * 60 * 2);
            //ServiceProvider = serviceProvider;
            
        }

        public override Tuple<List<Item>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Item> Query = this.DbContext.Items.AsNoTracking();
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name", "ArticleRealizationOrder"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "_id", "dataDestination[0].code", "dataDestination[0].name", "dataDestination[0].ArticleRealizationOrder", "dataDestination[0].color", "dataDestination[0].ImagePath", "dataDestination[0].ImgFile"
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

            List<Item> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }


        //public Tuple<List<Item>, int, Dictionary<string, string>, List<string>> GetRo(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        //{
        //    IQueryable<Item> Query = this.DbContext.Items.AsNoTracking();
        //    Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
        //    Query = ConfigureFilter(Query, FilterDictionary);
        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

        //    /* Search With Keyword */
        //    if (Keyword != null)
        //    {
        //        List<string> SearchAttributes = new List<string>()
        //        {
        //           "ArticleRealizationOrder"
        //        };

        //        Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
        //    }

        //    /* Const Select */
        //    List<string> SelectedFields = new List<string>()
        //    {
        //        "_id", "code", "name", "ArticleRealizationOrder"
        //    };



        //    /* Order */
        //    if (OrderDictionary.Count.Equals(0))
        //    {
        //        OrderDictionary.Add("_updatedDate", General.DESCENDING);

        //        Query = Query.OrderByDescending(b => b._LastModifiedUtc); /* Default Order */
        //    }
        //    else
        //    {
        //        string Key = OrderDictionary.Keys.First();
        //        string OrderType = OrderDictionary[Key];
        //        string TransformKey = General.TransformOrderBy(Key);

        //        BindingFlags IgnoreCase = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

        //        Query = OrderType.Equals(General.ASCENDING) ?
        //            Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
        //            Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
        //    }

        //    /* Pagination */
        //    //Pageable<Product> pageable = new Pageable<Product>(Query, Page - 1, Size);

        //    var totalData = Query.Count();
        //    Query = Query.Skip((Page - 1) * Size).Take(Size);

        //    List<Item> Data = Query.ToList();

        //    //int TotalData = Query.TotalCount;

        //    return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        //}

        public ItemViewModel MapToViewModel(Item item)
        {
            ItemViewModel itemVM = new ItemViewModel();


            int itemId = Convert.ToInt32(item.Id);
            itemVM._id = itemId;
            itemVM.UId = item.UId;
            itemVM._deleted = item._IsDeleted;
            itemVM._active = item.Active;
            itemVM._createdDate = item._CreatedUtc;
            itemVM._createdBy = item._CreatedBy;
            itemVM._createAgent = item._CreatedAgent;
            itemVM._updatedDate = item._LastModifiedUtc;
            itemVM._updatedBy = item._LastModifiedBy;
            itemVM._updateAgent = item._LastModifiedAgent;
            //itemVM.dataDestination.code = item.Code;
            //itemVM.dataDestination.name = item.Name;
            //itemVM.dataDestination.Uom = item.Uom;
            //itemVM.dataDestination.Size = item.Size;
            itemVM.DomesticRetail = item.DomesticRetail;
            itemVM.DomesticCOGS = item.DomesticCOGS;
            itemVM.DomesticSale = item.DomesticSale;
            itemVM.DomesticWholesale = item.DomesticWholesale;
            itemVM.InternationalSale = item.InternationalSale;
            itemVM.InternationalWholesale = item.InternationalWholesale;
            itemVM.InternationalRetail = item.InternatioalRetail;
            itemVM.InternationalCOGS = item.InternatinalCOGS;
            itemVM.price = item.DomesticCOGS;

            itemVM.dataDestination = new List<ItemViewModelRead>{
                new ItemViewModelRead {
                    ArticleRealizationOrder = item.ArticleRealizationOrder,
                    code = item.Code,
                    name = item.Name,
                    Uom = item.Uom,
                    Size = item.Size,
                    ImagePath = item.ImagePath,
                    ImgFile = item.ImgFile,
                    color = item.ColorDocName
                    
                }
            };
            //itemVM.dataDestination.ArticleRealizationOrder = item.ArticleRealizationOrder;
            itemVM.process = new ItemArticleProcesViewModel
            {
                _id = item.ArticleProcessId,
                code = item.ProcessDocCode,
                name = item.ProcessDocName
            };
            itemVM.materials = new ItemArticleMaterialViewModel
            {
                _id = item.ArticleMaterialsId,
                code = item.MaterialDocCode,
                name = item.MaterialDocName
            };
            itemVM.materialCompositions = new ItemArticleMaterialCompositionViewModel
            {
                _id = item.ArticleMaterialCompositionsId,
                code = item.MaterialCompositionDocCode,
                name = item.MaterialCompositionDocName
            };
            itemVM.collections = new ItemArticleCollectionViewModel
            {
                _id = item.ArticleSeasonsId,
                code = item.CollectionDocCode,
                name = item.CollectionDocName
            };
            itemVM.seasons = new ItemArticleSeasonViewModel
            {
                _id = item.ArticleSeasonsId,
                code = item.SeasonDocCode,
                name = item.SeasonDocName
            };
            itemVM.counters = new ItemArticleCounterViewModel
            {
                _id = item.ArticleCountersId,
                code = item.CounterDocCode,
                name = item.CounterDocName
            };
            itemVM.subCounters = new ItemArticleSubCounterViewModel
            {
                _id = item.ArticleSubCountersId,
                code = item.StyleDocCode,
                name = item.StyleDocName
            };

            itemVM.categories = new ItemArticleCategoryViewModel
            {
                _id = item.ArticleCategoriesId,
                code = item.CategoryDocCode,
                name = item.CategoryDocName
            };

            //itemVM.color = new ItemArticleColorViewModel
            //{
            //    _id = item.ArticleColorsId,
            //    code = item.ColorCode,
            //    name = item.ColorDocName
            //};

            ////itemVM.ProcessDocName = item.ProcessDocName;
            //itemVM.MaterialDocName = item.MaterialDocName;
            //itemVM.MaterialCompositionDocName = item.MaterialCompositionDocName;
            //itemVM.CollectionDocName = item.CollectionDocName;
            //itemVM.SeasonDocName = item.SeasonDocName;
            //itemVM.CounterDocName = item.CounterDocName;
            //itemVM.StyleDocName = item.StyleDocName;
            //itemVM.CategoryDocName = item.CategoryDocName;



            return itemVM;
        }

        public Item MapToModel(ItemViewModel itemVM)
        {
            Item item = new Item();

            item.Id = itemVM._id;
            item.UId = itemVM.UId;
            item._IsDeleted = itemVM._deleted;
            item.Active = itemVM._active;
            item._CreatedUtc = itemVM._createdDate;
            item._CreatedBy = itemVM._createdBy;
            item._CreatedAgent = itemVM._createAgent;
            item._LastModifiedUtc = itemVM._updatedDate;
            item._LastModifiedBy = itemVM._updatedBy;
            item._LastModifiedAgent = itemVM._updateAgent;
            item.Code = itemVM.dataDestination[0].code;
            item.Name = itemVM.dataDestination[0].name;
            item.Uom = itemVM.dataDestination[0].Uom;
            item.Size = itemVM.dataDestination[0].Size;
            item.DomesticCOGS = itemVM.DomesticCOGS;
            item.DomesticSale = itemVM.DomesticSale;
            item.InternationalSale = itemVM.InternationalSale;
            item.ArticleRealizationOrder = itemVM.dataDestination[0].ArticleRealizationOrder;
            if (!Equals(itemVM.process, null))
            {
                item.ArticleProcessId = itemVM.process._id;
                item.ProcessDocCode = itemVM.process.code;
                item.ProcessDocName = itemVM.process.name;
            }
            else
            {
                item.ArticleProcessId = 0;
                item.ProcessDocCode = null;
            }

            if (!Equals(itemVM.color, null))
            {
                item.ColorCode = itemVM.color.code;
                item.ArticleColorsId = itemVM.color._id;
                item.ColorDocName = itemVM.color.name;
            }

            if (!Equals(itemVM.materials, null))
            {
                item.ArticleMaterialsId = itemVM.materials._id;
                item.MaterialDocCode = itemVM.materials.code;
                item.MaterialDocName = itemVM.materials.name;
            }
            else
            {
                item.ArticleMaterialsId = 0;
                item.MaterialDocCode = null;
            }

            if (!Equals(itemVM.materialCompositions, null))
            {
                item.ArticleMaterialCompositionsId = itemVM.materialCompositions._id;
                item.MaterialCompositionDocCode = itemVM.materialCompositions.code;
                item.MaterialCompositionDocName = itemVM.materialCompositions.name;
            }
            else
            {
                item.ArticleMaterialCompositionsId = 0;
                item.MaterialCompositionDocCode = null;
            }

            if (!Equals(itemVM.collections, null))
            {
                item.ArticleCollectionsId = itemVM.collections._id;
                item.CollectionDocCode = itemVM.collections.code;
                item.CollectionDocName = itemVM.collections.name;
            }
            else
            {
                item.ArticleCollectionsId = 0;
                item.CollectionDocCode = null;
            }
            if (!Equals(itemVM.collections, null))
            {
                item.ArticleCollectionsId = itemVM.collections._id;
                item.CollectionDocCode = itemVM.collections.code;
                item.CollectionDocName = itemVM.collections.name;
            }
            else
            {
                item.ArticleCollectionsId = 0;
                item.CollectionDocCode = null;
            }

            if (!Equals(itemVM.seasons, null))
            {
                item.ArticleSeasonsId = itemVM.seasons._id;
                item.SeasonDocCode = itemVM.seasons.code;
                item.SeasonDocName = itemVM.seasons.name;
            }
            else
            {
                item.ArticleSeasonsId = 0;
                item.SeasonDocCode = null;
            }

            if (!Equals(itemVM.counters, null))
            {
                item.ArticleCountersId = itemVM.counters._id;
                item.CounterDocCode = itemVM.counters.code;
                item.CounterDocName = itemVM.counters.name;
            }
            else
            {
                item.ArticleCountersId = 0;
                item.CounterDocCode = null;
            }

            if (!Equals(itemVM.subCounters, null))
            {
                item.ArticleSubCountersId = itemVM.subCounters._id;
                item.StyleDocCode = itemVM.subCounters.code;
                item.StyleDocName = itemVM.subCounters.name;
            }
            else
            {
                item.ArticleSubCountersId = 0;
                item.StyleDocCode = null;
            }

            if (!Equals(itemVM.categories, null))
            {
                item.ArticleCategoriesId = itemVM.categories._id;
                item.CategoryDocCode = itemVM.categories.code;
                item.CategoryDocName = itemVM.categories.name;
            }
            else
            {
                item.ArticleCategoriesId = 0;
                item.CategoryDocCode = null;
            }
            item.ImagePath = itemVM.dataDestination[0].ImagePath;

            //item.ProcessDocName = itemVM.ProcessDocName;
            //item.MaterialDocName = item.MaterialDocName;
            //item.MaterialCompositionDocName = itemVM.MaterialCompositionDocName;
            //item.CollectionDocName = itemVM.CollectionDocName;
            //item.SeasonDocName = itemVM.SeasonDocName;
            //item.CounterDocName = itemVM.CounterDocName;
            //item.StyleDocName = itemVM.StyleDocName;
            //item.CategoryDocName = itemVM.CategoryDocName;

            return item;
        }

        public Item ItemMapToModel(ItemViewModelUsername itemVM)
        {
            Item item = new Item();

            item.Id = itemVM._id;
            item.UId = itemVM.UId;
            item._IsDeleted = itemVM._deleted;
            item.Active = itemVM._active;
            item._CreatedUtc = itemVM._createdDate;
            item._CreatedBy = itemVM._createdBy;
            item._CreatedAgent = itemVM._createAgent;
            item._LastModifiedUtc = itemVM._updatedDate;
            item._LastModifiedBy = itemVM._updatedBy;
            item._LastModifiedAgent = itemVM._updateAgent;
            item.Code = itemVM.dataDestination[0].code;
            item.Name = itemVM.dataDestination[0].name;
            item.Uom = itemVM.dataDestination[0].Uom;
            item.Size = itemVM.dataDestination[0].Size;
            item.DomesticCOGS = itemVM.DomesticCOGS;
            item.DomesticSale = itemVM.DomesticSale;
            item.InternationalSale = itemVM.InternationalSale;
            item.ArticleRealizationOrder = itemVM.dataDestination[0].ArticleRealizationOrder;
            item.TotalQty = itemVM.TotalQty;
            if (!Equals(itemVM.process, null))
            {
                item.ArticleProcessId = itemVM.process._id;
                item.ProcessDocCode = itemVM.process.code;
                item.ProcessDocName = itemVM.process.name;
            }
            else
            {
                item.ArticleProcessId = 0;
                item.ProcessDocCode = null;
            }

            if (!Equals(itemVM.color, null))
            {
                item.ColorCode = itemVM.color.code;
                item.ArticleColorsId = itemVM.color._id;
                item.ColorDocName = itemVM.color.name;
            }

            if (!Equals(itemVM.materials, null))
            {
                item.ArticleMaterialsId = itemVM.materials._id;
                item.MaterialDocCode = itemVM.materials.code;
                item.MaterialDocName = itemVM.materials.name;
            }
            else
            {
                item.ArticleMaterialsId = 0;
                item.MaterialDocCode = null;
            }

            if (!Equals(itemVM.materialCompositions, null))
            {
                item.ArticleMaterialCompositionsId = itemVM.materialCompositions._id;
                item.MaterialCompositionDocCode = itemVM.materialCompositions.code;
                item.MaterialCompositionDocName = itemVM.materialCompositions.name;
            }
            else
            {
                item.ArticleMaterialCompositionsId = 0;
                item.MaterialCompositionDocCode = null;
            }

            if (!Equals(itemVM.collections, null))
            {
                item.ArticleCollectionsId = itemVM.collections._id;
                item.CollectionDocCode = itemVM.collections.code;
                item.CollectionDocName = itemVM.collections.name;
            }
            else
            {
                item.ArticleCollectionsId = 0;
                item.CollectionDocCode = null;
            }
            if (!Equals(itemVM.collections, null))
            {
                item.ArticleCollectionsId = itemVM.collections._id;
                item.CollectionDocCode = itemVM.collections.code;
                item.CollectionDocName = itemVM.collections.name;
            }
            else
            {
                item.ArticleCollectionsId = 0;
                item.CollectionDocCode = null;
            }

            if (!Equals(itemVM.seasons, null))
            {
                item.ArticleSeasonsId = itemVM.seasons._id;
                item.SeasonDocCode = itemVM.seasons.code;
                item.SeasonDocName = itemVM.seasons.name;
            }
            else
            {
                item.ArticleSeasonsId = 0;
                item.SeasonDocCode = null;
            }

            if (!Equals(itemVM.counters, null))
            {
                item.ArticleCountersId = itemVM.counters._id;
                item.CounterDocCode = itemVM.counters.code;
                item.CounterDocName = itemVM.counters.name;
            }
            else
            {
                item.ArticleCountersId = 0;
                item.CounterDocCode = null;
            }

            if (!Equals(itemVM.subCounters, null))
            {
                item.ArticleSubCountersId = itemVM.subCounters._id;
                item.StyleDocCode = itemVM.subCounters.code;
                item.StyleDocName = itemVM.subCounters.name;
            }
            else
            {
                item.ArticleSubCountersId = 0;
                item.StyleDocCode = null;
            }

            if (!Equals(itemVM.categories, null))
            {
                item.ArticleCategoriesId = itemVM.categories._id;
                item.CategoryDocCode = itemVM.categories.code;
                item.CategoryDocName = itemVM.categories.name;
            }
            else
            {
                item.ArticleCategoriesId = 0;
                item.CategoryDocCode = null;
            }
            item.ImagePath = itemVM.dataDestination[0].ImagePath;

            //item.ProcessDocName = itemVM.ProcessDocName;
            //item.MaterialDocName = item.MaterialDocName;
            //item.MaterialCompositionDocName = itemVM.MaterialCompositionDocName;
            //item.CollectionDocName = itemVM.CollectionDocName;
            //item.SeasonDocName = itemVM.SeasonDocName;
            //item.CounterDocName = itemVM.CounterDocName;
            //item.StyleDocName = itemVM.StyleDocName;
            //item.CategoryDocName = itemVM.CategoryDocName;

            return item;
        }

        public override async Task<Item> ReadModelById(int Id)
        {
            //base.DbContext.Set<ProductSPPProperty>().Load();
            //return 
            //    DbContext.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            Item item = DbContext.Items.FirstOrDefault(x => x.Id == Id && x._IsDeleted.Equals(false));
            if (!string.IsNullOrWhiteSpace(item.ImagePath))
            {
                item.ImgFile = await this.AzureImageService.DownloadImage(item.GetType().Name, item.ImagePath);
            }
            return item;

        }

        public Task<List<ItemViewModel>> GetRO1( string RO)
        {

            var item = DbContext.Items.Where(x => x.ArticleRealizationOrder == RO);
            return item.Select(x => new ItemViewModel()
            {
                _id = x.Id,
                dataDestination = new List<ItemViewModelRead>() { new ItemViewModelRead() { code = x.Code, name = x.Name, ArticleRealizationOrder = x.ArticleRealizationOrder, _id = x.Id, Description = x.Description, ImagePath = x.ImagePath, Remark = x.Remark, Size = x.Size, Tags = x.Tags, Uom = x.Uom } },
                DomesticCOGS = x.DomesticCOGS,
                DomesticRetail = x.DomesticRetail,
                DomesticSale = x.DomesticSale,
                DomesticWholesale = x.DomesticWholesale,
                InternationalCOGS = x.InternatinalCOGS,
                InternationalRetail = x.InternatioalRetail,
                InternationalSale = x.InternationalSale,
                InternationalWholesale = x.InternationalWholesale,
                price = 0
            }).ToListAsync();
        }

        public Task<List<ItemLoader>> GetRO2(string RO)
        {

            var item = DbContext.Items.Where(x => x.ArticleRealizationOrder == RO);
            return item.Select(x => new ItemLoader()
            {
                //code = x.Code,
                //name = x.Name,
                //ArticleRealizationOrder = x.ArticleRealizationOrder,
                //_id = x.Id,
                //Description = x.Description,
                //ImagePath = x.ImagePath,
                //Remark = x.Remark,
                //Size = x.Size,
                //Tags = x.Tags,
                //Uom = x.Uom,

                dataDestination = new ItemViewModelRead { code = x.Code, name = x.Name, ArticleRealizationOrder = x.ArticleRealizationOrder, _id = x.Id, Description = x.Description, ImagePath = x.ImagePath, Remark = x.Remark, Size = x.Size, Tags = x.Tags, Uom = x.Uom },
                DomesticCOGS = x.DomesticCOGS,
                DomesticRetail = x.DomesticRetail,
                DomesticSale = x.DomesticSale,
                DomesticWholesale = x.DomesticWholesale,
                InternationalCOGS = x.InternatinalCOGS,
                InternationalRetail = x.InternatioalRetail,
                InternationalSale = x.InternationalSale,
                InternationalWholesale = x.InternationalWholesale,
                //price = 0
            }).ToListAsync();
        }
        public ItemViewModel GetDataInven(string ro, string code, string name)
        {
            var data = DbSet.Where(x => x.ArticleRealizationOrder.Equals(ro) && x.Name.Equals(name) && x.Code.Equals(code)).FirstOrDefault();
            return MapToViewModel(data);
        }
        public Task<List<ItemViewModelRead>> GetCode2(string code)
        {

            var item = DbContext.Items.Where(x => x.Code == code);
            return item.Select(x => new ItemViewModelRead()
            {
                code = x.Code,
                name = x.Name,
                ArticleRealizationOrder = x.ArticleRealizationOrder,
                _id = x.Id,
                Description = x.Description,
                ImagePath = x.ImagePath,
                Remark = x.Remark,
                Size = x.Size,
                Tags = x.Tags,
                Uom = x.Uom,
                DomesticCOGS = x.DomesticCOGS,
                DomesticRetail = x.DomesticRetail,
                DomesticSale = x.DomesticSale,
                DomesticWholesale = x.DomesticWholesale,
                InternationalCOGS = x.InternatinalCOGS,
                InternationalRetail = x.InternatioalRetail,
                InternationalSale = x.InternationalSale,
                InternationalWholesale = x.InternationalWholesale,

            }).ToListAsync();
        }

        public Task<List<ItemViewModel>> GetCode(string code)
        {

            var item = DbContext.Items.Where(x => x.Code == code);
            return item.Select(x => new ItemViewModel()
            {
                _id = x.Id,
                dataDestination = new List<ItemViewModelRead>() { new ItemViewModelRead() { code = x.Code, name = x.Name } },
                //code = x.Code,
                //name = x.Name,
                price = x.DomesticSale

            }).ToListAsync();
        }

        public ItemViewModel GetByCode(string code)
        {
            var item = DbContext.Items.Where(y => y.Code == code);
            return item.Select(y => new ItemViewModel()
            {
                _id = y.Id,
                //    price = y.DomesticSale

            }).FirstOrDefault();
        }

        private IAzureImageService AzureImageService
        {
            get { return this.ServiceProvider.GetService<IAzureImageService>(); }
        }

        public new async Task<int> UpdateAsync(int id, Item model)
        {
            await itemService.UpdateAsync(id, model);
            if (!string.IsNullOrWhiteSpace(model.ImagePath))
            {
                model.ImagePath = await this.AzureImageService.UploadImage(model.GetType().Name, model.Id, model._CreatedUtc, model.ImagePath);
            }
            return await DbContext.SaveChangesAsync();
        }


        //public ReadResponse<ItemViewModel> GetImage(string imagePath, string Uid)
        //{
        //    var Query = DbContext.Items.Where(x => x.UId == Uid && x.ImagePath == imagePath );

        //    //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    //if (OrderDictionary.Count.Equals(0))
        //    //{
        //    //    Query = Query.OrderByDescending(a => a.ReceiptDate).ThenByDescending(a => a.ReceiptDate);
        //    //}

        //    Pageable<ItemViewModel> pageable = new Pageable<ItemViewModel>(Query, page - 1, size);
        //    List<ItemViewModel> Data = pageable.Data.ToList<ItemViewModel>();
        //    int TotalData = pageable.TotalCount;

        //    return new ReadResponse<ItemViewModel>(Data, TotalData, OrderDictionary);
        //}

        public Task<List<ItemViewModel>> GetImage(string imagePath)
        {

            var item = DbContext.Items.Where(x =>  x.ImagePath == imagePath);
            return item.Select(x => new ItemViewModel()
            {
                _id = x.Id,
                dataDestination = new List<ItemViewModelRead>() { new ItemViewModelRead() { code = x.Code, name = x.Name, ImagePath = x.ImagePath } },
                //code = x.Code,
                //name = x.Name,
                //ImagePath = x.ImagePath,
                UId = x.UId
            }).ToListAsync();
        }

        //public async Task UploadImg(Item model)
        //{

        //    if (!string.IsNullOrWhiteSpace(model.ImagePath))
        //    {
        //        model.ImagePath = await this.AzureImageService.UploadImage(model.GetType().Name, model.Id, model._CreatedUtc, model.ImagePath);
        //    }
        //    return await DbContext.SaveChangesAsync();
        //}

        public virtual async Task<int> ImgPost(ItemViewModel model)
        { 
            Item model2 = new Item();
            string IMagePath = "";
            if (!string.IsNullOrWhiteSpace(model.ImageFile))
            {
                IMagePath = await this.AzureImageService.UploadImage(model2.GetType().Name, model._id, model._createdDate, model.ImageFile);
            }

            foreach (var data in model.dataDestination)
            {
                //Item newest = DbContext.Items.Where(x => x.Id == model._id).FirstOrDefault();
                //newest.
                //Item Items = new Item()
                //{
                //    Id = data._id,
                //    ArticleProcessId = model.process._id,
                //    ProcessDocCode = model.process.code,
                //    ProcessDocName = model.process.name,
                //    ArticleMaterialCompositionsId = model.materialCompositions._id,
                //    MaterialCompositionDocCode = model.materialCompositions.code,
                //    MaterialCompositionDocName = model.materialCompositions.name,
                //    ArticleCollectionsId = model.collections._id,
                //    CollectionDocCode = model.collections.code,
                //    CollectionDocName = model.collections.name,
                //    ArticleSeasonsId = model.seasons._id,
                //    SeasonDocCode = model.seasons.code,
                //    SeasonDocName = model.seasons.name,
                //    ArticleCountersId = model.counters._id,
                //    CounterDocCode = model.counters.code,
                //    CounterDocName = model.counters.name,
                //    ArticleSubCountersId = model.subCounters._id,
                //    StyleDocCode = model.subCounters.code,
                //    StyleDocName = model.subCounters.name,
                //    ImagePath = IMagePath,
                //    ImageFile = model.ImageFile,
                //    ArticleRealizationOrder = data.ArticleRealizationOrder,
                //    Code = data.code,
                //    Name = data.name,
                //    ArticleCategoriesId = newest.ArticleCategoriesId,
                //    ArticleColorsId = newest.ArticleColorsId,
                //    ArticleMaterialsId = newest
                //};
                Item ItemOld = DbContext.Items.Where(x => x.Id == data._id).FirstOrDefault();
                ItemOld.ArticleProcessId = model.process._id;
                ItemOld.ProcessDocCode = model.process.code;
                ItemOld.ProcessDocName = model.process.name;
                ItemOld.ArticleMaterialsId = model.materials._id;
                ItemOld.MaterialDocCode = model.materials.code;
                ItemOld.MaterialDocName = model.materials.name;
                ItemOld.ArticleMaterialCompositionsId = model.materialCompositions._id;
                ItemOld.MaterialCompositionDocCode = model.materialCompositions.code;
                ItemOld.MaterialCompositionDocName = model.materialCompositions.name;
                ItemOld.ArticleCollectionsId = model.collections._id;
                ItemOld.CollectionDocCode = model.collections.code;
                ItemOld.CollectionDocName = model.collections.name;
                ItemOld.ArticleSeasonsId = model.seasons._id;
                ItemOld.SeasonDocCode = model.seasons.code;
                ItemOld.SeasonDocName = model.seasons.name;
                ItemOld.ArticleCountersId = model.counters._id;
                ItemOld.CounterDocCode = model.counters.code;
                ItemOld.CounterDocName = model.counters.name;
                ItemOld.ArticleSubCountersId = model.subCounters._id;
                ItemOld.StyleDocCode = model.subCounters.code;
                ItemOld.StyleDocName = model.subCounters.name;
                ItemOld.ArticleColorsId = model.color._id;
                ItemOld.ColorCode = model.color.code;
                ItemOld.ColorDocName = model.color.name;
                ItemOld.ArticleCategoriesId = model.categories._id;
                ItemOld.CategoryDocCode = model.categories.code;
                ItemOld.CategoryDocName = model.categories.name;
                ItemOld.ImagePath = IMagePath;
                //ItemOld.ImgFile = model.ImageFile;

                DbSet.Update(ItemOld);


            }

            return await DbContext.SaveChangesAsync();
        }
        //return await this.AzureImageService.UploadImage(model.GetType().Name, model.Id, model._CreatedUtc, model.ImagePath);


        public async Task<int> UpdateTotalQtyAsync(int id, double totalQty, string username)
        {
            Item item = DbContext.Items.Where(y => y.Id == id).FirstOrDefault();
            if(item != null)
            {
                item.TotalQty = item.TotalQty + totalQty;
            }
            item.FlagForUpdate(username, "core-service");
            DbContext.Items.Update(item);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> ReduceTotalQtyAsync(int id, double totalQty, string username)
        {
            Item item = DbContext.Items.Where(y => y.Id == id).FirstOrDefault();
            if(item != null)
            {
                item.TotalQty = item.TotalQty - totalQty;
            }
            item.FlagForUpdate(username, "core-service");
            DbContext.Items.Update(item);
            return await DbContext.SaveChangesAsync();
        }
    }


}
