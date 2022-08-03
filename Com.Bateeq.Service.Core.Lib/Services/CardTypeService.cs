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
    public class CardTypeService : BasicService<CoreDbContext, CardType>, IMap<CardType, CardTypeViewModel>
    {
        public CardTypeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<CardType>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<CardType> Query = this.DbContext.CardTypes;
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
                .Select(b => new CardType
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
            Pageable<CardType> pageable = new Pageable<CardType>(Query, Page - 1, Size);
            List<CardType> Data = pageable.Data.ToList<CardType>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public CardTypeViewModel MapToViewModel(CardType cardType)
        {
           CardTypeViewModel cardTypeView = new CardTypeViewModel();

            cardTypeView._id = cardType.Id;
            cardTypeView.UId = cardType.Uid;
            cardTypeView._deleted = cardType._IsDeleted;
            cardTypeView._active = cardType.Active;
            cardTypeView._createdDate = cardType._CreatedUtc;
            cardTypeView._createdBy = cardType._CreatedBy;
            cardTypeView._createAgent = cardType._CreatedAgent;
            cardTypeView._updatedDate = cardType._LastModifiedUtc;
            cardTypeView._updatedBy = cardType._LastModifiedBy;
            cardTypeView._updateAgent = cardType._LastModifiedAgent;
            cardTypeView.code = cardType.Code;
            cardTypeView.description = cardType.Description;
            cardTypeView.name = cardType.Name;




            return cardTypeView;
        }

        public CardType MapToModel(CardTypeViewModel cardTypeView)
        {
            CardType cardType = new CardType();

            cardType.Id = cardTypeView._id;
            cardType.Uid = cardTypeView.UId;
            cardType._IsDeleted = cardTypeView._deleted;
            cardType.Active = cardTypeView._active;
            cardType._CreatedUtc = cardTypeView._createdDate;
            cardType._CreatedBy = cardTypeView._createdBy;
            cardType._CreatedAgent = cardTypeView._createAgent;
            cardType._LastModifiedUtc = cardTypeView._updatedDate;
            cardType._LastModifiedBy = cardTypeView._updatedBy;
            cardType._LastModifiedAgent = cardTypeView._updateAgent;
            cardType.Code = cardTypeView.code;
            cardType.Description = cardTypeView.description;
            cardType.Name = cardTypeView.name;

            return cardType;
        }
    }
}
