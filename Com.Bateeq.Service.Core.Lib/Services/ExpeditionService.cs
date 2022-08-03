using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels.Expedition;
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
    public class ExpeditionService : BasicService<CoreDbContext, MasterExpedition>, IMap<MasterExpedition, ExpeditionViewModel>
    {
        public ExpeditionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<MasterExpedition>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MasterExpedition> Query = this.DbContext.MasterExpeditions;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                     "Code","Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                 "_id", "Code", "Description", "Name"
            };

            Query = Query
                .Select(b => new MasterExpedition
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    Description = b.Description
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
            Pageable<MasterExpedition> pageable = new Pageable<MasterExpedition>(Query, Page - 1, Size);
            List<MasterExpedition> Data = pageable.Data.ToList<MasterExpedition>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ExpeditionViewModel MapToViewModel(MasterExpedition expedition)
        {
            ExpeditionViewModel expeditionVM = new ExpeditionViewModel();

            expeditionVM._id = expedition.Id;
            expeditionVM.UId = expedition.UId;
            expeditionVM._deleted = expedition._IsDeleted;
            expeditionVM._active = expedition.Active;
            expeditionVM._createdDate = expedition._CreatedUtc;
            expeditionVM._createdBy = expedition._CreatedBy;
            expeditionVM._createAgent = expedition._CreatedAgent;
            expeditionVM._updatedDate = expedition._LastModifiedUtc;
            expeditionVM._updatedBy = expedition._LastModifiedBy;
            expeditionVM._updateAgent = expedition._LastModifiedAgent;
            expeditionVM.code = expedition.Code;
            expeditionVM.description = expedition.Description;
            expeditionVM.name = expedition.Name;

            


            return expeditionVM;
        }

        public MasterExpedition MapToModel(ExpeditionViewModel expeditionVM)
        {
            MasterExpedition expedition = new MasterExpedition();

            expedition.Id = expeditionVM._id;
            expedition.UId = expeditionVM.UId;
            expedition._IsDeleted = expeditionVM._deleted;
            expedition.Active = expeditionVM._active;
            expedition._CreatedUtc = expeditionVM._createdDate;
            expedition._CreatedBy = expeditionVM._createdBy;
            expedition._CreatedAgent = expeditionVM._createAgent;
            expedition._LastModifiedUtc = expeditionVM._updatedDate;
            expedition._LastModifiedBy = expeditionVM._updatedBy;
            expedition._LastModifiedAgent = expeditionVM._updateAgent;
            expedition.Code = expeditionVM.code;
            expedition.Description = expeditionVM.description;
            expedition.Name = expeditionVM.name;

            return expedition;
        }

        public ExpeditionViewModel GetbyCode (string code)
        {
            var model = DbSet.Where(x => x.Code == code).FirstOrDefault();
            var viewmodel = MapToViewModel(model);
            return viewmodel;
        }



    }
}
