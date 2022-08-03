using Com.Danliris.Service.Core.Mongo.MongoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Mongo.MongoRepositories
{
    public interface ICurrencyMongoRepository
    {
        Task<IEnumerable<CurrencyMongo>> GetByBatch(int startingNumber, int numberOfBatch);
    }
}
