using Com.Danliris.Service.Core.Mongo.MongoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Mongo.MongoRepositories
{
    public interface IUnitOfMeasurementMongoRepository
    {
        Task<IEnumerable<UnitOfMeasurementMongo>> GetByBatch(int startingNumber, int numberOfBatch);
    }
}
