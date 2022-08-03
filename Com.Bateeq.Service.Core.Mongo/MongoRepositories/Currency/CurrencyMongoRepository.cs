using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Danliris.Service.Core.Mongo.MongoModels;
using MongoDB.Driver;

namespace Com.Danliris.Service.Core.Mongo.MongoRepositories
{
    public class CurrencyMongoRepository : ICurrencyMongoRepository
    {
        private readonly IMongoDbContext _context;

        public CurrencyMongoRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurrencyMongo>> GetByBatch(int startingNumber, int numberOfBatch)
        {
            return await _context
                            .Currencies
                            .Find(_ => true)
                            .Skip(startingNumber)
                            .Limit(numberOfBatch)
                            .ToListAsync();
        }
    }
}
