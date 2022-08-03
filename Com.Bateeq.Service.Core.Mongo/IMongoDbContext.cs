using Com.Danliris.Service.Core.Mongo.MongoModels;
using MongoDB.Driver;

namespace Com.Danliris.Service.Core.Mongo
{
    public interface IMongoDbContext
    {
        IMongoCollection<UnitOfMeasurementMongo> UnitOfMeasurements { get; }
        IMongoCollection<CurrencyMongo> Currencies { get; }
        IMongoCollection<ProductMongo> Products { get; }
    }
}
