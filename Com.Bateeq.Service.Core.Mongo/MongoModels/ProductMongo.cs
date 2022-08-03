using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Core.Mongo.MongoModels
{
    public class ProductMongo : MongoBaseModel
    {
        public string code { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public ObjectId currencyId { get; set; }
        public CurrencyMongo currency { get; set; }
        public string description { get; set; }
        public ObjectId uomId { get; set; }
        public UnitOfMeasurementMongo uom { get; set; }
        public string tags { get; set; }
        public dynamic properties { get; set; }
    }
}
