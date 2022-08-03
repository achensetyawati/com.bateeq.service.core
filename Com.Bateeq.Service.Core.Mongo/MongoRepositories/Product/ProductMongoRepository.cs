using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Core.Mongo.MongoModels;
using MongoDB.Driver;

namespace Com.Danliris.Service.Core.Mongo.MongoRepositories
{
    public class ProductMongoRepository : IProductMongoRepository
    {
        private readonly IMongoDbContext _context;

        public ProductMongoRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductMongo>> GetByBatch(int startingNumber, int numberOfBatch)
        {
            return await _context
                            .Products
                            .Find(_ => true)
                            .Skip(startingNumber)
                            .Limit(numberOfBatch)
                            .ToListAsync();
        }
    }
}
