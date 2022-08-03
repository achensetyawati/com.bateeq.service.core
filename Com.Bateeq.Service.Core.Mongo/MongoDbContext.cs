using Com.Danliris.Service.Core.Mongo.MongoModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Com.Danliris.Service.Core.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IOptions<MongoDbSettings> options, IMongoClient client)
        {
            _db = client.GetDatabase(options.Value.Database);
        }

        public IMongoCollection<UnitOfMeasurementMongo> UnitOfMeasurements => _db.GetCollection<UnitOfMeasurementMongo>("unit-of-measurements");

        public IMongoCollection<CurrencyMongo> Currencies => _db.GetCollection<CurrencyMongo>("currencies");

        public IMongoCollection<ProductMongo> Products => _db.GetCollection<ProductMongo>("products");
    }
}
