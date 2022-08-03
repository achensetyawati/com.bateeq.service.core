using Com.Danliris.Service.Core.Mongo.MongoModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Mongo.MongoRepositories
{
    public class UnitOfMeasurementMongoRepository : IUnitOfMeasurementMongoRepository
    {
        private readonly IMongoDbContext _context;

        public UnitOfMeasurementMongoRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnitOfMeasurementMongo>> GetByBatch(int startingNumber, int numberOfBatch)
        {
            return await _context
                            .UnitOfMeasurements
                            .Find(_ => true)
                            .Skip(startingNumber)
                            .Limit(numberOfBatch)
                            .ToListAsync();
        }
    }
}
